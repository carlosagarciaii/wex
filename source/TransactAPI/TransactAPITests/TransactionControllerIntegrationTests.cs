using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TransactAPI.Models;
using TransactAPI.Services;
using Xunit;

namespace TransactAPI.Tests
{
    public class TransactionControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly List<string> _testTransactionIds = new();

        public TransactionControllerIntegrationTests()
        {
            var factory = new TestWebApplicationFactory();  // No fixture needed
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetHelp_ReturnsValidResponse()
        {
            // Act
            var response = await _client.GetAsync("/tran/help");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Start Date Only", content);
            Assert.Contains("Start Date And End Date", content);
        }

        [Fact]
        public async Task GetCoffee_Returns418()
        {
            // Act
            var response = await _client.GetAsync("/tran/coffee");

            // Assert
            Assert.Equal(418, (int)response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Tea", content);
        }

        [Fact]
        public async Task Post_ValidTransaction_ReturnsSuccess()
        {
            // Arrange
            TransactionDataModel transaction = new()
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Transaction",
                PurchaseTotal = 100.50,
                PurchaseDate = DateTime.Now,
                Currency = "Japan-Yen"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(transaction),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsJsonAsync<TransactionDataModel>("/tran", transaction);
//            var response = await _client.PostAsync("/tran", content);

            // Assert
//            response.EnsureSuccessStatusCode();
            // var responseContent = await response.Content.ReadAsStringAsync();
            // Assert.Contains("\"code\":0", responseContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            _testTransactionIds.Add(transaction.ID);
        }

        [Fact]
        public async Task Get_WithValidDateRange_ReturnsTransactions()
        {
            // First post a transaction
            var transaction = new
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Query Transaction",
                PurchaseTotal = 200.75,
                PurchaseDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Currency = "USD"
            };

            var postContent = new StringContent(
                JsonSerializer.Serialize(transaction),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var postResponse = await _client.PostAsync("/tran", postContent);
            postResponse.EnsureSuccessStatusCode();
            _testTransactionIds.Add(transaction.ID);

            // Wait a moment for the transaction to be saved
            await Task.Delay(100);

            // Now get transactions for today
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            var getResponse = await _client.GetAsync($"/tran?startDate={startDate:yyyy-MM-dd}");

            getResponse.EnsureSuccessStatusCode();
            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<TransactionDataModel>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(transactions);
            Assert.NotEmpty(transactions);
        }

        [Fact]
        public async Task Post_WithDuplicateID_ReturnsConflict()
        {
            // Arrange
            var transactionId = Guid.NewGuid().ToString();
            TransactionDataModel transaction = new()
            {
                ID = transactionId,
                Description = "Duplicate Test",
                PurchaseTotal = 300.25,
                PurchaseDate = DateTime.Now,
                Currency = "Canada-Dollar"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(transaction),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // First post should succeed
            var firstResponse = await _client.PostAsync("/tran", content);
            firstResponse.EnsureSuccessStatusCode();
            _testTransactionIds.Add(transactionId);

            // Second post should fail with conflict
            var secondResponse = await _client.PostAsync("/tran", content);
            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Get_WithStartDateAfterEndDate_ReturnsBadRequest()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            // Act
            var response = await _client.GetAsync($"/tran?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Start Date Must be earlier than End Date", content);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

    }




    public class CurrencyServiceIntegrationTests
    {
        [Fact]
        public async Task GetExchangeRateAsync_ValidCurrency_ReturnsRate()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            // Act
            var result = await service.GetExchangeRateAsync("Mexico-Peso", new DateTime(2024, 1, 1));

            // Assert
            Assert.NotNull(result);
            Assert.True(result > 0);
        }

        [Fact]
        public async Task GetExchangeRateAsync_InvalidCurrency_ReturnsNull()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            // Act
            var result = await service.GetExchangeRateAsync("Invalid-Currency-123", new DateTime(2024, 1, 1));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetExchangeRateWithDetailsAsync_ValidCurrency_ReturnsRateAndDate()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            // Act
            var result = await service.GetExchangeRateWithDetailsAsync("Mexico-Peso", new DateTime(2024, 1, 1));

            // Assert
            Assert.NotNull(result.Rate);
            Assert.NotEmpty(result.RecordDate);
        }
    }
/*

    public class MariaDBConnIntegrationTests : IAsyncLifetime
    {
        private readonly MariaDBConn _dbConn;
        private readonly List<string> _testTransactionIds = new();

        public MariaDBConnIntegrationTests()
        {
            _dbConn = MariaDBConn.GetConnection(
                host: Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
                port: int.Parse(Environment.GetEnvironmentVariable("DB_PORT") ?? "3306"),
                dbName: Environment.GetEnvironmentVariable("DB_NAME") ?? "transact_test",
                user: Environment.GetEnvironmentVariable("DB_USER") ?? "test_user",
                password: Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "test_password"
            );
        }

        [Fact]
        public async Task GetTransactions_ReturnsData()
        {
            // Arrange - insert test data
            var testTransaction = new TransactionDataModel
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Integration Test Transaction",
                PurchaseTotal = 150.75,
                USDPurchaseTotal = 150.75,
                PurchaseDate = DateTime.Now,
                Currency = "USD"
            };

            await _dbConn.SaveTransactions(testTransaction);
            _testTransactionIds.Add(testTransaction.ID);

            // Act
            var results = await _dbConn.GetTransactions(
                DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            );

            // Assert
            Assert.NotNull(results);
            Assert.Contains(results, t => t.ID == testTransaction.ID);
        }

        [Fact]
        public async Task SaveTransactions_ValidTransaction_ReturnsSuccess()
        {
            // Arrange
            var transaction = new TransactionDataModel
            {
                ID = Guid.NewGuid().ToString(),
                Description = "Test Save",
                PurchaseTotal = 99.99,
                USDPurchaseTotal = 99.99,
                PurchaseDate = DateTime.Now,
                Currency = "USD"
            };

            // Act
            var result = await _dbConn.SaveTransactions(transaction);

            // Assert
            Assert.Equal(0, result.Code);
            Assert.NotEmpty(result.Message);

            _testTransactionIds.Add(transaction.ID);
        }

        [Fact]
        public async Task SaveTransactions_DuplicateID_ReturnsConflict()
        {
            // Arrange
            var transactionId = Guid.NewGuid().ToString();
            var transaction = new TransactionDataModel
            {
                ID = transactionId,
                Description = "Duplicate Test",
                PurchaseTotal = 100,
                USDPurchaseTotal = 100,
                PurchaseDate = DateTime.Now,
                Currency = "USD"
            };

            // First save
            await _dbConn.SaveTransactions(transaction);
            _testTransactionIds.Add(transactionId);

            // Second save should return conflict
            var result = await _dbConn.SaveTransactions(transaction);

            // Assert
            Assert.Equal(1, result.Code); // Assuming 1 is conflict
        }

        public async Task InitializeAsync()
        {
            await CleanupTestData();
        }

        public async Task DisposeAsync()
        {
            await CleanupTestData();
            await _dbConn.DisposeAsync();
        }

        private async Task CleanupTestData()
        {
            if (_testTransactionIds.Any())
            {
                // This would require a DeleteTransaction method in MariaDBConn
                // For now, we'll need to manually clean up or use a test database
                // that gets reset between test runs
            }
        }
    }

// */

}