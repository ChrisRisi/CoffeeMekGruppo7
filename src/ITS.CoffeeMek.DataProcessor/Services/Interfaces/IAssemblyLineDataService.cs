using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces;

public interface IAssemblyLineDataService
{
    Task AddAssemblyLineTelemetryAsync(AssemblyLine assemblyLine);
}
