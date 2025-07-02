using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class LatheEndpointsMap
    {
        public static IEndpointRouteBuilder MapLatheEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/lathes")
                .WithOpenApi()
                .WithTags("Lathes");

            group.MapGet("/", GetLathesAsync);
            group.MapGet("/{machineid}", GetLatheAsync);
            group.MapGet("/{machineid}/history", GetLatheHistoryAsync);
            group.MapPost("/", PostLatheAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Lathe>>> GetLathesAsync(ILatheDataService dataService)
        {
            var lathes = await dataService.GetLathesTelemetryAsync();
            return TypedResults.Ok(lathes);
        }

        private static async Task<Results<Ok<Lathe>, NotFound>> GetLatheAsync(int machineid, ILatheDataService dataService)
        {
            var lathe = await dataService.GetLatheTelemetryByMachineIdAsync(machineid);
            if (lathe is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(lathe);
        }

        private static async Task<Accepted> PostLatheAsync(LatheDataQueueMessage message, IQueueService queueService, IConfiguration configuration)
        {
            var queueName = Environment.GetEnvironmentVariable("LatheQueue") ?? configuration["ServiceBus:LatheQueue"];
            await queueService.SendMessageAsync(queueName, message);
            return TypedResults.Accepted($"/api/lathes/{message.MachineId}");
        }

        private static async Task<Ok<IEnumerable<Lathe>>> GetLatheHistoryAsync(int machineid, ILatheDataService dataService)
        {
            var history = await dataService.GetLatheHistoryAsync(machineid);
            return TypedResults.Ok(history);
        }
    }
}
