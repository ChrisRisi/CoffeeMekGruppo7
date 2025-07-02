using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class MillingMachineEndpointsMap
    {
        public static IEndpointRouteBuilder MapMillingMachineEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/millingmachines")
                .WithOpenApi()
                .WithTags("MillingMachines");

            group.MapGet("/", GetMillingMachinesAsync);
            group.MapGet("/{machineid}", GetMillingMachineAsync);
            group.MapGet("/{machineid}/history", GetMillingMachineHistoryAsync);
            group.MapPost("/", PostMillingMachineAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<MillingMachine>>> GetMillingMachinesAsync(IMillingMachineDataService dataService)
        {
            var millingMachines = await dataService.GetMillingMachinesTelemetryAsync();
            return TypedResults.Ok(millingMachines);
        }

        private static async Task<Results<Ok<MillingMachine>, NotFound>> GetMillingMachineAsync(int machineid, IMillingMachineDataService dataService)
        {
            var millingMachine = await dataService.GetMillingMachineTelemetryByMachineIdAsync(machineid);
            if (millingMachine is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(millingMachine);
        }

        private static async Task<Accepted> PostMillingMachineAsync(MillingMachineDataQueueMessage message, IQueueService queueService, IConfiguration configuration)
        {
            var queueName = Environment.GetEnvironmentVariable("MillingQueue") ?? configuration["ServiceBus:MillingQueue"];
            await queueService.SendMessageAsync(queueName, message);
            return TypedResults.Accepted($"/api/millingmachines/{message.MachineId}");
        }

        private static async Task<Ok<IEnumerable<MillingMachine>>> GetMillingMachineHistoryAsync(int machineId, IMillingMachineDataService dataService)
        {
            var history = await dataService.GetMillingMachineHistoryAsync(machineId);
            return TypedResults.Ok(history);
        }
    }
}
