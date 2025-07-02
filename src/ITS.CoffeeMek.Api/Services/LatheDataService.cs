using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    public class LatheDataService : ILatheDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<LatheDataService> _logger;

        public LatheDataService(IDatabaseService db, ILogger<LatheDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Lathe>> GetLathesTelemetryAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 l.*, m.*, ms.*
                    FROM CoffeeMek.Lathes l
                    JOIN CoffeeMek.Machines m ON l.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON l.MachineStateId = ms.Id
                    ORDER BY l.Id";

                var telemetryDict = new Dictionary<int, Lathe>();
                var result = await connection.QueryAsync<Lathe, Machine, MachineState, Lathe>(
                    query,
                    (telemetry, machine, machineState) =>
                    {
                        if (!telemetryDict.TryGetValue(telemetry.Id, out var telemetryEntry))
                        {
                            telemetryEntry = telemetry;
                            telemetryEntry.Machine = machine;
                            telemetryEntry.MachineState = machineState;
                            telemetryDict.Add(telemetryEntry.Id, telemetryEntry);
                        }
                        return telemetryEntry;
                    },
                    splitOn: "Id,Id");
                return telemetryDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lathes telemetry");
                throw;
            }
        }

        public async Task<Lathe?> GetLatheTelemetryByMachineIdAsync(int machineid)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 l.*, m.*, ms.*
                    FROM CoffeeMek.Lathes l
                    JOIN CoffeeMek.Machines m ON l.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON l.MachineStateId = ms.Id
                    WHERE l.MachineId = @machineid";

                var result = await connection.QueryAsync<Lathe, Machine, MachineState, Lathe>(
                    query,
                    (telemetry, machine, machineState) =>
                    {
                        telemetry.Machine = machine;
                        telemetry.MachineState = machineState;
                        return telemetry;
                    },
                    new { machineid },
                    splitOn: "Id,Id");
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lathe with Machine ID {machineid}", machineid);
                throw;
            }
        }

        public async Task<int> InsertTelemetryAsync(Lathe lathe)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Lathes (
                        MachineId, MachineStateId, RotationSpeed, SpindleTemperature, 
                        LocalTimeStamp, UTCTimeStamp, LastMaintenance
                    )
                    VALUES (
                        @MachineId, @MachineStateId, @RotationSpeed, @SpindleTemperature, 
                        @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, lathe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lathe telemetry");
                throw;
            }
        }

        public async Task<IEnumerable<Lathe>> GetLatheHistoryAsync(int machineId)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT l.*, m.*, ms.*
                    FROM CoffeeMek.Lathes l
                    JOIN CoffeeMek.Machines m ON l.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON l.MachineStateId = ms.Id
                    WHERE l.MachineId = @machineId
                    ORDER BY l.LocalTimeStamp";

                var result = await connection.QueryAsync<Lathe, Machine, MachineState, Lathe>(
                    query,
                    (telemetry, machine, machineState) =>
                    {
                        telemetry.Machine = machine;
                        telemetry.MachineState = machineState;
                        return telemetry;
                    },
                    new { machineId },
                    splitOn: "Id,Id");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lathe history for Machine ID {machineId}", machineId);
                throw;
            }
        }
    }
}