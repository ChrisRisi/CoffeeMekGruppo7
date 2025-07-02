using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class SiteEndpointsMap
    {
        public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/sites")
                .WithOpenApi()
                .WithTags("Sites");

            group.MapGet("/", GetSitesAsync);
            group.MapGet("/{id}", GetSiteAsync);
            group.MapPost("/", InsertSiteAsync);
            group.MapPut("/{id}", UpdateSiteAsync);
            group.MapDelete("/{id}", DeleteSiteAsync);

            return app;
        }

        private static async Task<Ok<IEnumerable<Site>>> GetSitesAsync(ISiteDataService dataService)
        {
            var sites = await dataService.GetSitesAsync();
            return TypedResults.Ok(sites);
        }

        private static async Task<Results<Ok<Site>, NotFound>> GetSiteAsync(int id, ISiteDataService dataService)
        {
            var site = await dataService.GetSiteByIdAsync(id);
            if (site is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(site);
        }

        private static async Task<Results<Created<Site>, BadRequest>> InsertSiteAsync(Site site, ISiteDataService dataService)
        {
            try
            {
                var id = await dataService.InsertAsync(site);
                site.Id = id;
                return TypedResults.Created($"/api/sites/{id}", site);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound, BadRequest>> UpdateSiteAsync(int id, Site site, ISiteDataService dataService)
        {
            if (id != site.Id)
                return TypedResults.BadRequest();

            var existing = await dataService.GetSiteByIdAsync(id);
            if (existing is null)
                return TypedResults.NotFound();

            try
            {
                await dataService.UpdateAsync(site);
                return TypedResults.NoContent();
            }
            catch (Exception)
            {
                return TypedResults.BadRequest();
            }
        }

        private static async Task<Results<NoContent, NotFound>> DeleteSiteAsync(int id, ISiteDataService dataService)
        {
            var site = await dataService.GetSiteByIdAsync(id);
            if (site is null)
                return TypedResults.NotFound();

            await dataService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
    }
}