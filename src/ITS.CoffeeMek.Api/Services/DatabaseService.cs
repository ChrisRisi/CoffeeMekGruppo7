using System.Data;
using System.Data.SqlClient;
using ITS.CoffeeMek.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ITS.CoffeeMek.Api.Services
{
    /// <summary>
    /// SQL Server implementation of database service
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}