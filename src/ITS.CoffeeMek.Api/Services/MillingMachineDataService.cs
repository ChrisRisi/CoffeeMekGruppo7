using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    public class MillingMachineDataService : IMillingMachineDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<MillingMachineDataService> _logger;

        public MillingMachineDataService(IDatabaseService db, ILogger<MillingMachineDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<MillingMachine>> GetMillingMachinesTelemetryAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 mm.*, m.*, ms.*
                    FROM CoffeeMek.MillingMachines mm
                    JOIN CoffeeMek.Machines m ON mm.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON mm.MachineStateId = ms.Id
                    ORDER BY mm.Id";

                var telemetryDict = new Dictionary<int, MillingMachine>();
                var result = await connection.QueryAsync<MillingMachine, Machine, MachineState, MillingMachine>(
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
                _logger.LogError(ex, "Error fetching milling machines telemetry");
                throw;
            }
        }

        public async Task<MillingMachine?> GetMillingMachineTelemetryByMachineIdAsync(int machineid)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 mm.*, m.*, ms.*
                    FROM CoffeeMek.MillingMachines mm
                    JOIN CoffeeMek.Machines m ON mm.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON mm.MachineStateId = ms.Id
                    WHERE mm.MachineId = @machineid";

                var result = await connection.QueryAsync<MillingMachine, Machine, MachineState, MillingMachine>(
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
                _logger.LogError(ex, "Error fetching milling machine with Machine ID {machineid}", machineid);
                throw;
            }
        }

        public async Task<int> InsertTelemetryAsync(MillingMachine millingMachine)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.MillingMachines (
                        MachineId, MachineStateId, CycleTime, Vibration, 
                        LocalTimeStamp, UTCTimeStamp, LastMaintenance
                    )
                    VALUES (
                        @MachineId, @MachineStateId, @CycleTime, @Vibration, 
                        @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, millingMachine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating milling machine telemetry");
                throw;
            }
        }

        public async Task<IEnumerable<MillingMachine>> GetMillingMachineHistoryAsync(int machineId)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 mm.*, m.*, ms.*
                    FROM CoffeeMek.MillingMachines mm
                    JOIN CoffeeMek.Machines m ON mm.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON mm.MachineStateId = ms.Id
                    WHERE mm.MachineId = @machineId
                    ORDER BY mm.LocalTimeStamp";

                var result = await connection.QueryAsync<MillingMachine, Machine, MachineState, MillingMachine>(
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
                _logger.LogError(ex, "Error fetching milling machine history for Machine ID {machineId}", machineId);
                throw;
            }
        }
    }
}