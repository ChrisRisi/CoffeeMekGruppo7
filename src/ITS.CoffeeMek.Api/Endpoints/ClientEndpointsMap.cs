using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class ClientEndpointsMap
    {
        public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/clients")
                .WithOpenApi()
                .WithTags("Clients");

            group.MapGet("/", GetClientsAsync);
            group.MapGet("/{id}", GetClientAsync);
            group.MapPost("/", InsertClientAsync);
            group.MapPut("/{id}", UpdateClientAsync);
            group.MapDelete("/{id}", DeleteClientAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Client>>> GetClientsAsync(IClientDataService dataService)
        {
            var clients = await dataService.GetClientsAsync();
            return TypedResults.Ok(clients);
        }

        private static async Task<Results<Ok<Client>, NotFound>> GetClientAsync(int id, IClientDataService dataService)
        {
            var client = await dataService.GetClientByIdAsync(id);
            if (client is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(client);
        }

        private static async Task<Results<Created<Client>, BadRequest>> InsertClientAsync(Client client, IClientDataService dataService)
        {
            try
            {
                var id = await dataService.InsertAsync(client);
                client.Id = id;
                return TypedResults.Created($"/api/clients/{id}", client);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateClientAsync(int id, Client client, IClientDataService dataService)
        {
            if (id != client.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetClientByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(client);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteClientAsync(int id, IClientDataService dataService)
        {
            var client = await dataService.GetClientByIdAsync(id);
            if (client is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
    }
}