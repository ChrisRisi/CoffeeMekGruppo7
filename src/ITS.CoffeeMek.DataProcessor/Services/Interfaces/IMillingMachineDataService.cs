using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces;

public interface IMillingMachineDataService
{
    Task AddMillingMachineTelemetryAsync(MillingMachine millingMachine);
}
