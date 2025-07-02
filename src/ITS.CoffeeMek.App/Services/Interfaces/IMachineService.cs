using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IMachineService
    {
        Task<IEnumerable<Machine>?> GetMachinesAsync();
        Task<Machine?> GetMachineByIdAsync(int id);
        Task<Machine?> AddMachineAsync(Machine machine);
        Task UpdateMachineAsync(Machine machine);
        Task DeleteMachineAsync(int id);
    }
}
