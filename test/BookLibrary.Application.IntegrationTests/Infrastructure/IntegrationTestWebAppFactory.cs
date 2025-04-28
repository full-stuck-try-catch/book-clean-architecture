
using System.Security.Claims;
using BookLibrary.Application.Abstractions.Data;
using BookLibrary.Application.IntegrationTests.Infrastructure;
using BookLibrary.Infrastructure;
using BookLibrary.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("BookLibraryDB1")
        .WithUsername("postgres")
        .WithPassword("1123456789")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            string connectionString = $"{_dbContainer.GetConnectionString()};Pooling=False";

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());

            services.RemoveAll(typeof(ISqlConnectionFactory));

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(connectionString));

            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

            // Create fake ClaimsPrincipal
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, TestData.TestUserId.ToString()),
                new Claim(ClaimTypes.Email, TestData.RegisterTestUserRequest.email)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var fakeHttpContext = new DefaultHttpContext
            {
                User = principal
            };

            // Replace IHttpContextAccessor
            IHttpContextAccessor accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext.Returns(fakeHttpContext);

            services.RemoveAll<IHttpContextAccessor>();
            services.AddSingleton(accessor);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
