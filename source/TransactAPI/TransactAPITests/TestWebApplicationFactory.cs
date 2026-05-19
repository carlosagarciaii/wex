using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactAPI.Services;

namespace TransactAPI.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Add test configuration
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["DB_HOST"] = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
                    ["DB_PORT"] = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306",
                    ["DB_NAME"] = Environment.GetEnvironmentVariable("DB_NAME") ?? "transact_test",
                    ["DB_USER"] = Environment.GetEnvironmentVariable("DB_USER") ?? "test_user",
                    ["DB_PASSWORD"] = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "test_password"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Replace MariaDBConn service registration
                services.AddScoped<MariaDBConn>(provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    return MariaDBConn.GetConnection(
                        host: config["DB_HOST"],
                        port: int.Parse(config["DB_PORT"]),
                        dbName: config["DB_NAME"],
                        user: config["DB_USER"],
                        password: config["DB_PASSWORD"]
                    );
                });
            });
        }
    }
}