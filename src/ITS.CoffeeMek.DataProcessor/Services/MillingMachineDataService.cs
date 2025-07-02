using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services;

public class MillingMachineDataService : IMillingMachineDataService
{
    private readonly DapperContext _context;

    public MillingMachineDataService(DapperContext context)
    {
        _context = context;
    }

    public async Task AddMillingMachineTelemetryAsync(MillingMachine millingMachine)
    {
        using var connection = _context.CreateConnection();
        const string query = @"
            INSERT INTO CoffeeMek.MillingMachines (
                MachineId, MachineStateId, CycleTime, Vibration, 
                LocalTimeStamp, UTCTimeStamp, LastMaintenance
            )
            VALUES (
                @MachineId, @MachineStateId, @CycleTime, @Vibration, 
                @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
            )";
        
        await connection.ExecuteAsync(query, millingMachine);
    }
}
