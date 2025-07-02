using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface ILatheService
    {
        Task<IEnumerable<Lathe>?> GetLathesAsync();
        Task<Lathe?> GetLatheByMachineIdAsync(int machineId);
        Task PostLatheAsync(LatheDataQueueMessage message);
        Task<IEnumerable<Lathe>> GetLatheHistoryAsync(int machineId);
    }
}
