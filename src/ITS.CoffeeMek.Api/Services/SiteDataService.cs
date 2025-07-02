using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    /// <summary>
    /// Implementation of Site data operations using Dapper and SQL Server
    /// </summary>
    public class SiteDataService : ISiteDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<SiteDataService> _logger;

        public SiteDataService(IDatabaseService db, ILogger<SiteDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Site>> GetSitesAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Name, Country, Address 
                    FROM CoffeeMek.Sites
                    ORDER BY Id";
                
                return await connection.QueryAsync<Site>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all sites");
                throw;
            }
        }

        public async Task<Site?> GetSiteByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Name, Country, Address 
                    FROM CoffeeMek.Sites
                    WHERE Id = @Id";
                
                return await connection.QueryFirstOrDefaultAsync<Site>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving site with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(Site site)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Sites (Name, Address, Country)
                    VALUES (@Name, @Address, @Country);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, site);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating site");
                throw;
            }
        }

        public async Task UpdateAsync(Site site)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    UPDATE CoffeeMek.Sites 
                    SET Name = @Name, 
                        Address = @Address, 
                        Country = @Country
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, site);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating site with ID {site.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"DELETE FROM CoffeeMek.Sites WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting site with ID {id}");
                throw;
            }
        }
    }
}