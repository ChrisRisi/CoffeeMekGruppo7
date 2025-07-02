using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces;
    public interface ITestingLineDataService
    {
        Task<IEnumerable<TestingLine>> GetTestingLinesTelemetryAsync();
        Task<TestingLine?> GetTestingLineByMachineIdAsync(int machineid);
        Task<int> InsertTelemetryAsync(TestingLine testingLine);
        Task<IEnumerable<TestingLine>> GetTestingLineHistoryAsync(int machineId);
    }