using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    public class AssemblyLineDataService : IAssemblyLineDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<AssemblyLineDataService> _logger;

        public AssemblyLineDataService(IDatabaseService db, ILogger<AssemblyLineDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<AssemblyLine>> GetAssemblyLinesTelemetryAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 al.*, m.*, ms.*
                    FROM CoffeeMek.AssemblyLine al
                    JOIN CoffeeMek.Machines m ON al.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON al.MachineStateId = ms.Id
                    ORDER BY al.Id";

                var telemetryDict = new Dictionary<int, AssemblyLine>();
                var result = await connection.QueryAsync<AssemblyLine, Machine, MachineState, AssemblyLine>(
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
                _logger.LogError(ex, "Error fetching assembly lines telemetry");
                throw;
            }
        }

        public async Task<AssemblyLine?> GetAssemblyLineTelemetryByMachineIdAsync(int machineid)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 al.*, m.*, ms.*
                    FROM CoffeeMek.AssemblyLine al
                    JOIN CoffeeMek.Machines m ON al.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON al.MachineStateId = ms.Id
                    WHERE al.MachineId = @machineid";

                var result = await connection.QueryAsync<AssemblyLine, Machine, MachineState, AssemblyLine>(
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
                _logger.LogError(ex, "Error fetching assembly line with Machine ID {machineid}", machineid);
                throw;
            }
        }

        public async Task<int> InsertTelemetryAsync(AssemblyLine assemblyLine)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.AssemblyLine (
                        MachineId, MachineStateId, AvgStationTime, OperatorCount, 
                        LocalTimeStamp, UTCTimeStamp, LastMaintenance
                    )
                    VALUES (
                        @MachineId, @MachineStateId, @AvgStationTime, @OperatorCount, 
                        @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, assemblyLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating assembly line telemetry");
                throw;
            }
        }

        public async Task<IEnumerable<AssemblyLine>> GetAssemblyLineHistoryAsync(int machineId)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 al.*, m.*, ms.*
                    FROM CoffeeMek.AssemblyLine al
                    JOIN CoffeeMek.Machines m ON al.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON al.MachineStateId = ms.Id
                    WHERE al.MachineId = @machineId
                    ORDER BY al.LocalTimeStamp";

                var result = await connection.QueryAsync<AssemblyLine, Machine, MachineState, AssemblyLine>(
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
                _logger.LogError(ex, "Error fetching assembly line history for Machine ID {machineId}", machineId);
                throw;
            }
        }
    }
}
