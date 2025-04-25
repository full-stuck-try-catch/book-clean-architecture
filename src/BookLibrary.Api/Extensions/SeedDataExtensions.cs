using System.Data;
using Bogus;
using BookLibrary.Application.Abstractions.Data;
using Dapper;

namespace BookLibrary.Api.Extensions;

internal static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();


        int adminRoleId = connection.QuerySingle<int>("SELECT id FROM roles WHERE name = @Name", new { Name = "Admin" });

        // Seed admin user
        var id = Guid.NewGuid();
        string adminEmail = "admin@yourdomain.com";
        string firstName = "admin";
        string lastName = "admin";
        string adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123"); // Use your password hashing here!
        DateTime registerAt = DateTime.UtcNow;

        Guid? adminUserId = connection.QueryFirstOrDefault<Guid?>(
            "SELECT id FROM users WHERE email = @Email", new { Email = adminEmail });

        if (adminUserId == null)
        {
            connection.Execute(
                "INSERT INTO users (id , email, first_name, last_name, password_hash, registered_at) VALUES (@Id, @Email, @FirstName , @LastName, @PasswordHash , @RegisteredAt)",
                new { Id = id, Email = adminEmail, FirstName = firstName, LastName = lastName, PasswordHash = adminPassword, RegisteredAt = registerAt });

            adminUserId = connection.QuerySingle<Guid>("SELECT id FROM users WHERE email = @Email", new { Email = adminEmail });
        }

        // Assign admin role to admin user
        int existsUserRole = connection.QueryFirstOrDefault<int>(
            "SELECT 1 FROM role_user WHERE users_id = @UserId AND roles_id = @RoleId",
            new { UserId = adminUserId, RoleId = adminRoleId });

        if (existsUserRole == 0)
        {
            connection.Execute(
                "INSERT INTO role_user (users_id, roles_id) VALUES (@UserId, @RoleId)",
                new { UserId = adminUserId, RoleId = adminRoleId });
        }
    }
}
