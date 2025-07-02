using System.Data;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for database connection services
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns>An open database connection</returns>
        IDbConnection CreateConnection();
    }
}