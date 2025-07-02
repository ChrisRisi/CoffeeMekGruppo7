using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    public interface ILatheDataService
    {
        Task<IEnumerable<Lathe>> GetLathesTelemetryAsync();
        Task<Lathe?> GetLatheTelemetryByMachineIdAsync(int machineid);
        Task<int> InsertTelemetryAsync(Lathe lathe);
        Task<IEnumerable<Lathe>> GetLatheHistoryAsync(int machineId);
    }
}
