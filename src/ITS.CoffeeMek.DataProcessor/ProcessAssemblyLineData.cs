using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITS.CoffeeMek.DataProcessor;

public class ProcessAssemblyLineData
{
    private readonly ILogger<ProcessAssemblyLineData> _logger;
    private readonly IAssemblyLineDataService _dataService;

    public ProcessAssemblyLineData(ILogger<ProcessAssemblyLineData> logger, IAssemblyLineDataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    [Function(nameof(ProcessAssemblyLineData))]
    public async Task Run([ServiceBusTrigger("assembly-data", Connection = "ServiceBusConnection")] AssemblyLineDataQueueMessage message)
    {
        _logger.LogInformation($"Message received from AssemblyQueue: MachineId {message.MachineId}");

        try
        {
            var assemblyLineTelemetry = new AssemblyLine
            {
                MachineId = message.MachineId,
                MachineStateId = message.MachineStateId,
                AvgStationTime = (decimal)message.AvgStationTime,
                OperatorCount = message.OperatorCount,
                LocalTimeStamp = message.LocalTimeStamp,
                UTCTimeStamp = message.UTCTimeStamp
            };

            await _dataService.AddAssemblyLineTelemetryAsync(assemblyLineTelemetry);

            _logger.LogInformation($"Successfully processed and saved data for MachineId {message.MachineId}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message for MachineId {message.MachineId}");
            throw;
        }
    }
}
