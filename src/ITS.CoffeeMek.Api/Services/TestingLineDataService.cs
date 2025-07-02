using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    public class TestingLineDataService : ITestingLineDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<TestingLineDataService> _logger;

        public TestingLineDataService(IDatabaseService db, ILogger<TestingLineDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<TestingLine>> GetTestingLinesTelemetryAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 tl.*, m.*, ms.*
                    FROM CoffeeMek.TestingLine tl
                    JOIN CoffeeMek.Machines m ON tl.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON tl.MachineStateId = ms.Id
                    ORDER BY tl.Id";

                var telemetryDict = new Dictionary<int, TestingLine>();
                var result = await connection.QueryAsync<TestingLine, Machine, MachineState, TestingLine>(
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
                _logger.LogError(ex, "Error fetching testing lines telemetry");
                throw;
            }
        }

        public async Task<TestingLine?> GetTestingLineByMachineIdAsync(int machineid)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 tl.*, m.*, ms.*
                    FROM CoffeeMek.TestingLine tl
                    JOIN CoffeeMek.Machines m ON tl.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON tl.MachineStateId = ms.Id
                    WHERE tl.MachineId = @machineid";

                var result = await connection.QueryAsync<TestingLine, Machine, MachineState, TestingLine>(
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
                _logger.LogError(ex, "Error fetching testing line with Machine ID {machineid}", machineid);
                throw;
            }
        }

        public async Task<int> InsertTelemetryAsync(TestingLine testingLine)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.TestingLine (
                        MachineId, MachineStateId, BoilerPressure, BoilerTemperature, EnergyConsumption, 
                        LocalTimeStamp, UTCTimeStamp, LastMaintenance
                    )
                    VALUES (
                        @MachineId, @MachineStateId, @BoilerPressure, @BoilerTemperature, @EnergyConsumption, 
                        @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, testingLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating testing line telemetry");
                throw;
            }
        }

        public async Task<IEnumerable<TestingLine>> GetTestingLineHistoryAsync(int machineId)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT TOP 20 tl.*, m.*, ms.*
                    FROM CoffeeMek.TestingLine tl
                    JOIN CoffeeMek.Machines m ON tl.MachineId = m.Id
                    LEFT JOIN CoffeeMek.MachineStates ms ON tl.MachineStateId = ms.Id
                    WHERE tl.MachineId = @machineId
                    ORDER BY tl.LocalTimeStamp";

                var result = await connection.QueryAsync<TestingLine, Machine, MachineState, TestingLine>(
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
                _logger.LogError(ex, "Error fetching testing line history for Machine ID {machineId}", machineId);
                throw;
            }
        }
    }
}
