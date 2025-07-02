namespace ITS.CoffeeMek.Api.Endpoints
{
    public static class EndpointsMappingExtensions
    {
        public static void MapAllApiEndpoints(this WebApplication app)
        {
            app.MapSiteEndpoints();
            app.MapClientEndpoints();
            app.MapOrderEndpoints();
            app.MapMachineEndpoints();
            app.MapMachineStateEndpoints();
            app.MapLotEndpoints();
            app.MapAssemblyLineEndpoints();
            app.MapLatheEndpoints();
            app.MapMillingMachineEndpoints();
            app.MapTestingLineEndpoints();
        }
    }
}