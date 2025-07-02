using ITS.CoffeeMek.Api.Services;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class OrderEndpointsMap
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/orders")
                .WithOpenApi()
                .WithTags("Orders");

            group.MapGet("/", GetOrdersAsync);
            group.MapGet("/open", GetOpenOrdersAsync);
            group.MapGet("/{id}", GetOrderAsync);
            group.MapPost("/", InsertOrderAsync);
            group.MapPut("/{id}", UpdateOrderAsync);
            group.MapDelete("/{id}", DeleteOrderAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Order>>> GetOrdersAsync(IOrderDataService dataService)
        {
            var orders = await dataService.GetOrdersAsync();
            return TypedResults.Ok(orders);
        }

        private static async Task<Ok<IEnumerable<Order>>> GetOpenOrdersAsync(IOrderDataService dataService)
        {
            var orders = await dataService.GetOpenOrdersAsync();
            return TypedResults.Ok(orders);
        }

        private static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int id, IOrderDataService dataService)
        {
            var order = await dataService.GetOrderByIdAsync(id);
            if (order is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(order);
        }

        private static async Task<Results<Created<Order>, BadRequest>> InsertOrderAsync(Order order, IOrderDataService orderDataService, IOrderFulfillmentService orderFulfillmentService)
        {
            try
            {
                if (order.CreationTime == default)
                {
                    order.CreationTime = DateTime.UtcNow;
                }
                
                var id = await orderDataService.InsertAsync(order);
                order.Id = id;

                await orderFulfillmentService.ProcessOrderFulfillmentAsync();

                return TypedResults.Created($"/api/orders/{id}", order);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateOrderAsync(int id, Order order, IOrderDataService dataService)
        {
            if (id != order.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetOrderByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(order);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(int id, IOrderDataService dataService)
        {
            var order = await dataService.GetOrderByIdAsync(id);
            if (order is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
    }
}