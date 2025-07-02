using ITS.CoffeeMek.Api.Services;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class LotEndpointsMap
    {
        public static IEndpointRouteBuilder MapLotEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/lots")
                .WithOpenApi()
                .WithTags("Lots");

            group.MapGet("/", GetLotsAsync);
            group.MapGet("/{id}", GetLotAsync);
            group.MapPost("/", InsertLotAsync);
            group.MapPut("/{id}", UpdateLotAsync);
            group.MapDelete("/{id}", DeleteLotAsync);
            group.MapGet("/site/{siteId:int}", GetLotsBySiteAsync);
            group.MapGet("/order/{orderId:int}", GetLotsByOrderAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Lot>>> GetLotsAsync(ILotDataService dataService)
        {
            var lots = await dataService.GetLotsAsync();
            return TypedResults.Ok(lots);
        }

        private static async Task<Results<Ok<Lot>, NotFound>> GetLotAsync(int id, ILotDataService dataService)
        {
            var lot = await dataService.GetLotByIdAsync(id);
            if (lot is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(lot);
        }

        private static async Task<Results<Created<Lot>, BadRequest>> InsertLotAsync(Lot lot, ILotDataService lotDataService, IOrderFulfillmentService orderFulfillmentService)
        {
            try
            {
                var id = await lotDataService.InsertAsync(lot);
                lot.Id = id;

                await orderFulfillmentService.ProcessOrderFulfillmentAsync();

                return TypedResults.Created($"/api/lots/{id}", lot);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateLotAsync(int id, Lot lot, ILotDataService dataService)
        {
            if (id != lot.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetLotByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(lot);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteLotAsync(int id, ILotDataService dataService)
        {
            var lot = await dataService.GetLotByIdAsync(id);
            if (lot is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }

        private static async Task<Ok<IEnumerable<Lot>>> GetLotsBySiteAsync(int siteId, ILotDataService dataService)
        {
            var lots = await dataService.GetLotsBySiteIdAsync(siteId);
            return TypedResults.Ok(lots);
        }

        private static async Task<Ok<IEnumerable<Lot>>> GetLotsByOrderAsync(int orderId, ILotDataService dataService)
        {
            var lots = await dataService.GetLotsByOrderIdAsync(orderId);
            return TypedResults.Ok(lots);
        }


    }
}