using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IAssemblyLineService
    {
        Task<IEnumerable<AssemblyLine>?> GetAssemblyLinesAsync();
        Task<AssemblyLine?> GetAssemblyLineByMachineIdAsync(int machineId);
        Task PostAssemblyLineAsync(AssemblyLineDataQueueMessage message);
        Task<IEnumerable<AssemblyLine>> GetAssemblyLineHistoryAsync(int machineId);
    }
}
