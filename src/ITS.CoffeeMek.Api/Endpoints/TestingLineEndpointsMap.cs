using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class TestingLineEndpointsMap
    {
        public static IEndpointRouteBuilder MapTestingLineEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/testinglines")
                .WithOpenApi()
                .WithTags("TestingLines");

            group.MapGet("/", GetTestingLinesAsync);
            group.MapGet("/{machineid}", GetTestingLineAsync);
            group.MapGet("/{machineid}/history", GetTestingLineHistoryAsync);
            group.MapPost("/", PostTestingLineAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<TestingLine>>> GetTestingLinesAsync(ITestingLineDataService dataService)
        {
            var testingLines = await dataService.GetTestingLinesTelemetryAsync();
            return TypedResults.Ok(testingLines);
        }

        private static async Task<Results<Ok<TestingLine>, NotFound>> GetTestingLineAsync(int machineid, ITestingLineDataService dataService)
        {
            var testingLine = await dataService.GetTestingLineByMachineIdAsync(machineid);
            if (testingLine is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(testingLine);
        }

        private static async Task<Accepted> PostTestingLineAsync(TestingLineDataQueueMessage message, IQueueService queueService, IConfiguration configuration)
        {
            var queueName = Environment.GetEnvironmentVariable("TestingQueue") ?? configuration["ServiceBus:TestingQueue"];
            if (!string.IsNullOrEmpty(queueName))
            {
                await queueService.SendMessageAsync(queueName, message);
            }

            return TypedResults.Accepted($"/api/testinglines/{message.MachineId}");
        }

        private static async Task<Ok<IEnumerable<TestingLine>>> GetTestingLineHistoryAsync(int machineid, ITestingLineDataService dataService)
        {
            var history = await dataService.GetTestingLineHistoryAsync(machineid);
            return TypedResults.Ok(history);
        }
    }
}
