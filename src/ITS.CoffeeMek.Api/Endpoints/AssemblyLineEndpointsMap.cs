using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class AssemblyLineEndpointsMap
    {
        public static IEndpointRouteBuilder MapAssemblyLineEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/assemblylines")
                .WithOpenApi()
                .WithTags("AssemblyLines");

            group.MapGet("/", GetAssemblyLinesAsync);
            group.MapGet("/{machineid}", GetAssemblyLineAsync);
            group.MapGet("/{machineid}/history", GetAssemblyLineHistoryAsync);
            group.MapPost("/", PostAssemblyLineAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<AssemblyLine>>> GetAssemblyLinesAsync(IAssemblyLineDataService dataService)
        {
            var assemblyLines = await dataService.GetAssemblyLinesTelemetryAsync();
            return TypedResults.Ok(assemblyLines);
        }

        private static async Task<Results<Ok<AssemblyLine>, NotFound>> GetAssemblyLineAsync(int machineid, IAssemblyLineDataService dataService)
        {
            var assemblyLine = await dataService.GetAssemblyLineTelemetryByMachineIdAsync(machineid);
            if (assemblyLine is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(assemblyLine);
        }

        private static async Task<Accepted> PostAssemblyLineAsync(AssemblyLineDataQueueMessage message, IQueueService queueService, IConfiguration configuration)
        {
            var queueName = Environment.GetEnvironmentVariable("AssemblyQueue") ?? configuration["ServiceBus:AssemblyQueue"];
            if (queueName is not null)
            {
                await queueService.SendMessageAsync(queueName, message);
            }

            return TypedResults.Accepted($"/api/assemblylines/{message.MachineId}");
        }

        private static async Task<Ok<IEnumerable<AssemblyLine>>> GetAssemblyLineHistoryAsync(int machineid, IAssemblyLineDataService dataService)
        {
            var history = await dataService.GetAssemblyLineHistoryAsync(machineid);
            return TypedResults.Ok(history);
        }
    }
}
