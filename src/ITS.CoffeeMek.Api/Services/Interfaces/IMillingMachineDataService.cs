using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    public interface IMillingMachineDataService
    {
        Task<IEnumerable<MillingMachine>> GetMillingMachinesTelemetryAsync();
        Task<MillingMachine?> GetMillingMachineTelemetryByMachineIdAsync(int machineid);
        Task<int> InsertTelemetryAsync(MillingMachine millingMachine);
        Task<IEnumerable<MillingMachine>> GetMillingMachineHistoryAsync(int machineId);
    }
}
