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
    /// Implementation of MachineState data operations using Dapper and SQL Server
    /// </summary>
    public class MachineStateDataService : IMachineStateDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<MachineStateDataService> _logger;

        public MachineStateDataService(IDatabaseService db, ILogger<MachineStateDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<MachineState>> GetMachineStatesAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Description, Error 
                    FROM CoffeeMek.MachineStates
                    ORDER BY Id";
                
                return await connection.QueryAsync<MachineState>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all machine states");
                throw;
            }
        }

        public async Task<MachineState?> GetMachineStateByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT Id, Description, Error 
                    FROM CoffeeMek.MachineStates
                    WHERE Id = @Id";
                
                return await connection.QueryFirstOrDefaultAsync<MachineState>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving machine state with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(MachineState machineState)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.MachineStates (Description, Error)
                    VALUES (@Description, @Error);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, machineState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating machine state");
                throw;
            }
        }

        public async Task UpdateAsync(MachineState machineState)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    UPDATE CoffeeMek.MachineStates 
                    SET Description = @Description, 
                        Error = @Error
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, machineState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating machine state with ID {machineState.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"DELETE FROM CoffeeMek.MachineStates WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting machine state with ID {id}");
                throw;
            }
        }
    }
}