using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IMillingMachineService
    {
        Task<IEnumerable<MillingMachine>?> GetMillingMachinesAsync();
        Task<MillingMachine?> GetMillingMachineByMachineIdAsync(int machineId);
        Task PostMillingMachineAsync(MillingMachineDataQueueMessage message);
        Task<IEnumerable<MillingMachine>> GetMillingMachineHistoryAsync(int machineId);
    }
}
