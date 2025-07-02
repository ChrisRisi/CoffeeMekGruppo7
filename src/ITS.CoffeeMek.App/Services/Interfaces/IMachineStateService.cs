using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IMachineStateService
    {
        Task<IEnumerable<MachineState>?> GetMachineStatesAsync();
        Task<MachineState?> GetMachineStateByIdAsync(int id);
        Task<MachineState?> AddMachineStateAsync(MachineState machineState);
        Task UpdateMachineStateAsync(MachineState machineState);
        Task DeleteMachineStateAsync(int id);
    }
}
