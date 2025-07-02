using ITS.CoffeeMek.Api.Services;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class MachineStateEndpointsMap
    {
        public static IEndpointRouteBuilder MapMachineStateEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/machinestates")
                .WithOpenApi()
                .WithTags("MachineStates");

            group.MapGet("/", GetMachineStatesAsync);
            group.MapGet("/{id}", GetMachineStateAsync);
            group.MapPost("/", InsertMachineStateAsync);
            group.MapPut("/{id}", UpdateMachineStateAsync);
            group.MapDelete("/{id}", DeleteMachineStateAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<MachineState>>> GetMachineStatesAsync(IMachineStateDataService dataService)
        {
            var machineStates = await dataService.GetMachineStatesAsync();
            return TypedResults.Ok(machineStates);
        }

        private static async Task<Results<Ok<MachineState>, NotFound>> GetMachineStateAsync(int id, IMachineStateDataService dataService)
        {
            var machineState = await dataService.GetMachineStateByIdAsync(id);
            if (machineState is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(machineState);
        }

        private static async Task<Results<Created<MachineState>, BadRequest>> InsertMachineStateAsync(MachineState machineState, IMachineStateDataService dataService)
        {
            try
            {
                var id = await dataService.InsertAsync(machineState);
                machineState.Id = id;
                return TypedResults.Created($"/api/machinestates/{id}", machineState);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateMachineStateAsync(int id, MachineState machineState, IMachineStateDataService dataService)
        {
            if (id != machineState.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetMachineStateByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(machineState);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteMachineStateAsync(int id, IMachineStateDataService dataService)
        {
            var machineState = await dataService.GetMachineStateByIdAsync(id);
            if (machineState is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
    }
}