using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services;

public class AssemblyLineDataService : IAssemblyLineDataService
{
    private readonly DapperContext _context;

    public AssemblyLineDataService(DapperContext context)
    {
        _context = context;
    }

    public async Task AddAssemblyLineTelemetryAsync(AssemblyLine assemblyLine)
    {
        using var connection = _context.CreateConnection();
        const string query = @"
            INSERT INTO CoffeeMek.AssemblyLine (
                MachineId, MachineStateId, AvgStationTime, OperatorCount, 
                LocalTimeStamp, UTCTimeStamp, LastMaintenance
            )
            VALUES (
                @MachineId, @MachineStateId, @AvgStationTime, @OperatorCount, 
                @LocalTimeStamp, @UTCTimeStamp, @LastMaintenance
            )";
        
        await connection.ExecuteAsync(query, assemblyLine);
    }
}
