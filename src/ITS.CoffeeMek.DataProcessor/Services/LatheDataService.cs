using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services;

public class LatheDataService : ILatheDataService
{
    private readonly DapperContext _context;

    public LatheDataService(DapperContext context)
    {
        _context = context;
    }

    public async Task AddLatheTelemetryAsync(Lathe lathe)
    {
        using var connection = _context.CreateConnection();
        const string query = @"
            INSERT INTO CoffeeMek.Lathes (
                MachineId, MachineStateId, RotationSpeed, SpindleTemperature, 
                LocalTimeStamp, UTCTimeStamp, LastMaintenance
            )
            VALUES (
                @MachineId, @MachineStateId, @RotationSpeed, @SpindleTemperature, 
                @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
            )";
        
        await connection.ExecuteAsync(query, lathe);
    }
}
