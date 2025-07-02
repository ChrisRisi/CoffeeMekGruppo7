using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITS.CoffeeMek.DataProcessor;

public class ProcessLatheData
{
    private readonly ILogger<ProcessLatheData> _logger;
    private readonly ILatheDataService _dataService;

    public ProcessLatheData(ILogger<ProcessLatheData> logger, ILatheDataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    [Function(nameof(ProcessLatheData))]
    public async Task Run([ServiceBusTrigger("lathe-data", Connection = "ServiceBusConnection")] LatheDataQueueMessage message)
    {
        _logger.LogInformation($"Message received from LatheQueue: MachineId {message.MachineId}");

        try
        {
            var latheTelemetry = new Lathe
            {
                MachineId = message.MachineId,
                MachineStateId = message.MachineStateId,
                RotationSpeed = (decimal)message.RotationSpeed,
                SpindleTemperature = (decimal)message.SpindleTemperature,
                LocalTimeStamp = message.LocalTimeStamp,
                UTCTimeStamp = message.UTCTimeStamp
            };

            await _dataService.AddLatheTelemetryAsync(latheTelemetry);

            _logger.LogInformation($"Successfully processed and saved data for MachineId {message.MachineId}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message for MachineId {message.MachineId}");
            throw;
        }
    }
}
