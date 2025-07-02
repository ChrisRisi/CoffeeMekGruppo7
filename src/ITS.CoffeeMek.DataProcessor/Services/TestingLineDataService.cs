using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services;

public class TestingLineDataService : ITestingLineDataService
{
    private readonly DapperContext _context;

    public TestingLineDataService(DapperContext context)
    {
        _context = context;
    }

    public async Task AddTestingLineTelemetryAsync(TestingLine testingLine)
    {
        using var connection = _context.CreateConnection();
        const string query = @"
            INSERT INTO CoffeeMek.TestingLine (
                MachineId, MachineStateId, BoilerPressure, BoilerTemperature, EnergyConsumption, 
                LocalTimeStamp, UTCTimeStamp, LastMaintenance
            )
            VALUES (
                @MachineId, @MachineStateId, @BoilerPressure, @BoilerTemperature, @EnergyConsumption, 
                @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
            )";
        
        await connection.ExecuteAsync(query, testingLine);
    }
}
