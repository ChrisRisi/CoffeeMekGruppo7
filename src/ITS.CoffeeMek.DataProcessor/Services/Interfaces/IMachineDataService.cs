using ITS.CoffeeMek.Models;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces
{
    public interface IMachineDataService
    {
        Task<Machine?> GetMachineByIdAsync(int machineId);
    }
}
