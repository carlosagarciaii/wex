using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TransactAPI.Services;

namespace TransactAPI.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        private readonly string _testDbConnectionString;

        public TestWebApplicationFactory()
        {
            // Setup test database connection
            _testDbConnectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
                                     $"Port={Environment.GetEnvironmentVariable("DB_PORT") ?? "3306"};" +
                                     $"Database={Environment.GetEnvironmentVariable("DB_NAME") ?? "transact_test"};" +
                                     $"Uid={Environment.GetEnvironmentVariable("DB_USER") ?? "test_user"};" +
                                     $"Pwd={Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "test_password"};" +
                                     "SslMode=none;Connection Timeout=20;GuidFormat=Binary16;";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing MariaDBConn registration if it exists
                // Since your original code doesn't use DI, we'll need to work differently

                // For integration tests, we'll use the real implementation
                // You might need to modify your Program.cs to support test configurations
            });
        }

        public MariaDBConn CreateDbConnection()
        {
            return MariaDBConn.GetConnection(
                host: Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
                port: int.Parse(Environment.GetEnvironmentVariable("DB_PORT") ?? "3306"),
                dbName: Environment.GetEnvironmentVariable("DB_NAME") ?? "transact_test",
                user: Environment.GetEnvironmentVariable("DB_USER") ?? "test_user",
                password: Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "test_password"
            );
        }

        public void Dispose() {
        }

    }
}