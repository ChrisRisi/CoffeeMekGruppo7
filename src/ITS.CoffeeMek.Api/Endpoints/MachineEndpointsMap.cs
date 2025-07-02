using ITS.CoffeeMek.Api.Services;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class MachineEndpointsMap
    {
        public static IEndpointRouteBuilder MapMachineEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/machines")
                .WithOpenApi()
                .WithTags("Machines");

            group.MapGet("/", GetMachinesAsync);
            group.MapGet("/{id}", GetMachineAsync);
            group.MapPost("/", InsertMachineAsync);
            group.MapPut("/{id}", UpdateMachineAsync);
            group.MapDelete("/{id}", DeleteMachineAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Machine>>> GetMachinesAsync(IMachineDataService dataService)
        {
            var machines = await dataService.GetMachinesAsync();
            return TypedResults.Ok(machines);
        }

        private static async Task<Results<Ok<Machine>, NotFound>> GetMachineAsync(int id, IMachineDataService dataService)
        {
            var machine = await dataService.GetMachineByIdAsync(id);
            if (machine is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(machine);
        }

        private static async Task<Results<Created<Machine>, BadRequest>> InsertMachineAsync(Machine machine, IMachineDataService dataService)
        {
            try
            {
                var id = await dataService.InsertAsync(machine);
                machine.Id = id;
                return TypedResults.Created($"/api/machines/{id}", machine);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateMachineAsync(int id, Machine machine, IMachineDataService dataService)
        {
            if (id != machine.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetMachineByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(machine);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteMachineAsync(int id, IMachineDataService dataService)
        {
            var machine = await dataService.GetMachineByIdAsync(id);
            if (machine is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
    }
}