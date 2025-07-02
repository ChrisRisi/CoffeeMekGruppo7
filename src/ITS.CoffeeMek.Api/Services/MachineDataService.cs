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
    /// Implementation of Machine data operations using Dapper and SQL Server
    /// </summary>
    public class MachineDataService : IMachineDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<MachineDataService> _logger;

        public MachineDataService(IDatabaseService db, ILogger<MachineDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Machine>> GetMachinesAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT m.Id, m.Name, m.SiteId, m.Address,
                           s.Id as SiteId, s.Name, s.Country, s.Address
                    FROM CoffeeMek.Machines m
                    JOIN CoffeeMek.Sites s ON m.SiteId = s.Id
                    ORDER BY m.Id";

                var machineDict = new Dictionary<int, Machine>();
                
                await connection.QueryAsync<Machine, Site, Machine>(
                    query,
                    (machine, site) =>
                    {
                        if (!machineDict.TryGetValue(machine.Id, out var machineEntry))
                        {
                            machineEntry = machine;
                            machineEntry.Site = site;
                            machineDict.Add(machineEntry.Id, machineEntry);
                        }
                        return machineEntry;
                    },
                    splitOn: "SiteId"
                );
                
                return machineDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all machines");
                throw;
            }
        }

        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT m.Id, m.Name, m.SiteId, m.Address,
                           s.Id as SiteId, s.Name, s.Country, s.Address
                    FROM CoffeeMek.Machines m
                    JOIN CoffeeMek.Sites s ON m.SiteId = s.Id
                    WHERE m.Id = @Id";

                var machineDict = new Dictionary<int, Machine>();
                
                await connection.QueryAsync<Machine, Site, Machine>(
                    query,
                    (machine, site) =>
                    {
                        if (!machineDict.TryGetValue(machine.Id, out var machineEntry))
                        {
                            machineEntry = machine;
                            machineEntry.Site = site;
                            machineDict.Add(machineEntry.Id, machineEntry);
                        }
                        return machineEntry;
                    },
                    new { Id = id },
                    splitOn: "SiteId"
                );
                
                return machineDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving machine with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(Machine machine)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Machines (Name, SiteId, Address)
                    VALUES (@Name, @SiteId, @Address);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, machine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating machine");
                throw;
            }
        }

        public async Task UpdateAsync(Machine machine)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    UPDATE CoffeeMek.Machines 
                    SET Name = @Name, 
                        SiteId = @SiteId, 
                        Address = @Address
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, machine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating machine with ID {machine.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"DELETE FROM CoffeeMek.Machines WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting machine with ID {id}");
                throw;
            }
        }
    }
}