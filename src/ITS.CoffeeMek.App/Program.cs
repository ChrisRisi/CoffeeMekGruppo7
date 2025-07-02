using ApexCharts;
using ITS.CoffeeMek.App.Services;
using ITS.CoffeeMek.App.Services.Interfaces;
using MudBlazor.Services;

namespace ITS.CoffeeMek.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddMudServices();
        builder.Services.AddApexCharts();

        var apiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl") ?? builder.Configuration["ApiBaseUrl"];

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

        builder.Services.AddScoped<ISiteService, SiteService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ILotService, LotService>();
        builder.Services.AddScoped<IMachineService, MachineService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IMachineStateService, MachineStateService>();
        builder.Services.AddScoped<IAssemblyLineService, AssemblyLineService>();
        builder.Services.AddScoped<ILatheService, LatheService>();
        builder.Services.AddScoped<IMillingMachineService, MillingMachineService>();
        builder.Services.AddScoped<ITestingLineService, TestingLineService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();

        // Fix: Ensure 'App' refers to a valid type, not a namespace
        app.MapRazorComponents<Components.App>() // Assuming 'App' is a component in the 'Components' namespace
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
