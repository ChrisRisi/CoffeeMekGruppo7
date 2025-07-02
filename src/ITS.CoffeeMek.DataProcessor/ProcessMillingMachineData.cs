using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITS.CoffeeMek.DataProcessor;

public class ProcessMillingMachineData
{
    private readonly ILogger<ProcessMillingMachineData> _logger;
    private readonly IMillingMachineDataService _dataService;

    public ProcessMillingMachineData(ILogger<ProcessMillingMachineData> logger, IMillingMachineDataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    [Function(nameof(ProcessMillingMachineData))]
    public async Task Run([ServiceBusTrigger("milling-data", Connection = "ServiceBusConnection")] MillingMachineDataQueueMessage message)
    {
        _logger.LogInformation($"Message received from MillingQueue: MachineId {message.MachineId}");

        try
        {
            var millingMachineTelemetry = new MillingMachine
            {
                MachineId = message.MachineId,
                MachineStateId = message.MachineStateId,
                CycleTime = (decimal)message.CycleTime,
                Vibration = (decimal)message.Vibration,
                LocalTimeStamp = message.LocalTimeStamp,
                UTCTimeStamp = message.UTCTimeStamp
            };

            await _dataService.AddMillingMachineTelemetryAsync(millingMachineTelemetry);

            _logger.LogInformation($"Successfully processed and saved data for MachineId {message.MachineId}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message for MachineId {message.MachineId}");
            throw;
        }
    }
}
