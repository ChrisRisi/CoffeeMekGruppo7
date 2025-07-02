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
    /// Implementation of Client data operations using Dapper and SQL Server
    /// </summary>
    public class ClientDataService : IClientDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<ClientDataService> _logger;

        public ClientDataService(IDatabaseService db, ILogger<ClientDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Name, Address 
                    FROM CoffeeMek.Clients
                    ORDER BY Id";
                
                return await connection.QueryAsync<Client>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all clients");
                throw;
            }
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Name, Address 
                    FROM CoffeeMek.Clients
                    WHERE Id = @Id";
                
                return await connection.QueryFirstOrDefaultAsync<Client>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving client with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(Client client)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Clients (Name, Address)
                    VALUES (@Name, @Address);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                throw;
            }
        }

        public async Task UpdateAsync(Client client)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    UPDATE CoffeeMek.Clients 
                    SET Name = @Name, 
                        Address = @Address
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating client with ID {client.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"DELETE FROM CoffeeMek.Clients WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting client with ID {id}");
                throw;
            }
        }
    }
}