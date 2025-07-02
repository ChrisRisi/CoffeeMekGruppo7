using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces;

public interface ILatheDataService
{
    Task AddLatheTelemetryAsync(Lathe lathe);
}
