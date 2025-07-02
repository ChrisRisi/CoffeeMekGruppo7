using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    public interface IAssemblyLineDataService
    {
        Task<IEnumerable<AssemblyLine>> GetAssemblyLinesTelemetryAsync();
        Task<AssemblyLine?> GetAssemblyLineTelemetryByMachineIdAsync(int machineid);
        Task<int> InsertTelemetryAsync(AssemblyLine assemblyLine);
        Task<IEnumerable<AssemblyLine>> GetAssemblyLineHistoryAsync(int machineId);
    }
}
