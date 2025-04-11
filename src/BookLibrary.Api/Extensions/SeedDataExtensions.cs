//using System.Data;
//using Bogus;
//using BookLibrary.Application.Abstractions.Data;
//using Dapper;

//namespace BookLibrary.Api.Extensions;

//internal static class SeedDataExtensions
//{
//    public static void SeedData(this IApplicationBuilder app)
//    {
//        using IServiceScope scope = app.ApplicationServices.CreateScope();

//        ISqlConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
//        using IDbConnection connection = sqlConnectionFactory.CreateConnection();

       

//        const string sql = """
            
//            """;

//        connection.Execute(sql, apartments);
//    }
//}
