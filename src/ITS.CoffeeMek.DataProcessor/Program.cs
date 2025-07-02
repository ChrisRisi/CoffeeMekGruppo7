using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Registra il DapperContext
        services.AddSingleton<DapperContext>();

        // Registra i servizi dati
        services.AddScoped<ILatheDataService, LatheDataService>();
        services.AddScoped<IMillingMachineDataService, MillingMachineDataService>();
        services.AddScoped<IAssemblyLineDataService, AssemblyLineDataService>();
        services.AddScoped<ITestingLineDataService, TestingLineDataService>();
        services.AddScoped<ILotDataService, LotDataService>();
        services.AddScoped<IOrderFulfillmentService, OrderFulfillmentService>();
        services.AddScoped<IMachineDataService, MachineDataService>();
        services.AddScoped<IOrderDataService, OrderDataService>();
    })
    .Build();

host.Run();
