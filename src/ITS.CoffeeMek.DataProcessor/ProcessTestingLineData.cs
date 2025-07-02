using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITS.CoffeeMek.DataProcessor;

public class ProcessTestingLineData
{
    private readonly ILogger<ProcessTestingLineData> _logger;
    private readonly ITestingLineDataService _testingLineDataService;
    private readonly ILotDataService _lotDataService;
    private readonly IMachineDataService _machineDataService;
    private readonly IOrderDataService _orderDataService;
    private readonly IOrderFulfillmentService _orderFulfillmentService;
    private const int TestPassedStateId = 6; // TODO: Confirm the correct ID for 'Test Passed' state

    public ProcessTestingLineData(
        ILogger<ProcessTestingLineData> logger, 
        ITestingLineDataService testingLineDataService,
        ILotDataService lotDataService,
        IMachineDataService machineDataService,
        IOrderDataService orderDataService,
        IOrderFulfillmentService orderFulfillmentService)
    {
        _logger = logger;
        _testingLineDataService = testingLineDataService;
        _lotDataService = lotDataService;
        _machineDataService = machineDataService;
        _orderDataService = orderDataService;
        _orderFulfillmentService = orderFulfillmentService;
    }

    [Function(nameof(ProcessTestingLineData))]
    public async Task Run([ServiceBusTrigger("testing-data", Connection = "ServiceBusConnection")] TestingLineDataQueueMessage message)
    {
        _logger.LogInformation($"Message received from TestingQueue: MachineId {message.MachineId}");

        try
        {
            var testingLineTelemetry = new TestingLine
            {
                MachineId = message.MachineId,
                MachineStateId = message.MachineStateId,
                BoilerPressure = (decimal)message.BoilerPressure,
                BoilerTemperature = (decimal)message.BoilerTemperature,
                EnergyConsumption = (decimal)message.EnergyConsumption,
                LocalTimeStamp = message.LocalTimeStamp,
                UTCTimeStamp = message.UTCTimeStamp
            };

            await _testingLineDataService.AddTestingLineTelemetryAsync(testingLineTelemetry);

            // Lot management logic starts here
            if (message.MachineStateId == TestPassedStateId)
            {
                await HandleSuccessfulTestAsync(message.MachineId);
            }

            _logger.LogInformation($"Successfully processed and saved data for MachineId {message.MachineId}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing message for MachineId {message.MachineId}");
            throw;
        }
    }

    private async Task HandleSuccessfulTestAsync(int machineId)
    {
        var machine = await _machineDataService.GetMachineByIdAsync(machineId);
        if (machine == null)
        {
            _logger.LogWarning($"Machine with ID {machineId} not found. Cannot process lot logic.");
            return;
        }

        var activeLot = await _lotDataService.GetActiveLotBySiteIdAsync(machine.SiteId);

        if (activeLot != null)
        {
            // An active lot exists, so we increment its quantity
            activeLot.CurrentQuantity++;
            _logger.LogInformation($"Incrementing lot {activeLot.Id}. New quantity: {activeLot.CurrentQuantity}");

            if (activeLot.CurrentQuantity >= activeLot.Capacity)
            {
                // The lot is full, so we close it
                activeLot.EndTimeStamp = DateTime.UtcNow;
                _logger.LogInformation($"Lot {activeLot.Id} is now full. Closing lot.");
            }

            await _lotDataService.UpdateLotAsync(activeLot);
            // await _orderFulfillmentService.ProcessOrderFulfillmentAsync();
        }
        else
        {
            // No active lot exists, we need to create a new one.
            _logger.LogInformation($"No active lot for SiteId {machine.SiteId}. Creating a new lot.");

            const int defaultCapacity = 50;

            var newLot = new Lot
            {
                OrderId = null, // No associated order as per business logic
                SiteId = machine.SiteId,
                Capacity = defaultCapacity,
                CurrentQuantity = 1, // Start with the first successful item
                StartTimeStamp = DateTime.UtcNow,
                EndTimeStamp = null, // The lot is active
                Code = $"LOT-{machine.SiteId}-{DateTime.UtcNow:yyyyMMddHHmmss}" // Generate a unique lot code
            };

            var createdLot = await _lotDataService.CreateLotAsync(newLot);
            _logger.LogInformation($"Successfully created new lot with Id {createdLot.Id} for SiteId {machine.SiteId}.");

            await _orderFulfillmentService.ProcessOrderFulfillmentAsync();
        }
    }
}
