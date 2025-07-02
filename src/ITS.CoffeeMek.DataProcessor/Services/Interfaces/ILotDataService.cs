using ITS.CoffeeMek.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces
{
    public interface ILotDataService
    {
        Task<Lot?> GetActiveLotBySiteIdAsync(int siteId);
        Task UpdateLotAsync(Lot lot);
        Task<Lot> CreateLotAsync(Lot lot);
        Task<IEnumerable<Lot>> GetAllLotsAsync();
        Task<IEnumerable<Lot>> GetLotsByOrderIdAsync(int orderId);
        Task<IEnumerable<Lot>> GetUnassignedLotsAsync();
    }
}
