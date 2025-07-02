using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces;

public interface ITestingLineDataService
{
    Task AddTestingLineTelemetryAsync(TestingLine testingLine);
}
