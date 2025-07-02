using ITS.CoffeeMek.Api.Endpoints;
using ITS.CoffeeMek.Api.Services;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Text.Json.Serialization;

namespace ITS.CoffeeMek.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            // Add database service
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // Register data services
            builder.Services.AddScoped<ISiteDataService, SiteDataService>();
            builder.Services.AddScoped<IClientDataService, ClientDataService>();
            builder.Services.AddScoped<IOrderDataService, OrderDataService>();
            builder.Services.AddScoped<IMachineDataService, MachineDataService>();
            builder.Services.AddScoped<IMachineStateDataService, MachineStateDataService>();
            builder.Services.AddScoped<ILotDataService, LotDataService>();
            builder.Services.AddScoped<ILatheDataService, LatheDataService>();
            builder.Services.AddScoped<IMillingMachineDataService, MillingMachineDataService>();
            builder.Services.AddScoped<IAssemblyLineDataService, AssemblyLineDataService>();
            builder.Services.AddScoped<ITestingLineDataService, TestingLineDataService>();
            builder.Services.AddScoped<IOrderFulfillmentService, OrderFulfillmentService>();

            // Register queue service
            builder.Services.AddSingleton<IQueueService, QueueService>();

            // Add authorization
            builder.Services.AddAuthorization();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            //if (app.Environment.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeMek API v1");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at application root
                });
            //}

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            // Map all endpoints
            app.MapAllApiEndpoints();

            app.Run();
        }
    }
}
