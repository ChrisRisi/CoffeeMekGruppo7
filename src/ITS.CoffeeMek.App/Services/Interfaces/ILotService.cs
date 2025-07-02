using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface ILotService
    {
        Task<IEnumerable<Lot>?> GetLotsAsync();
        Task<Lot?> GetLotByIdAsync(int id);
        Task<Lot?> AddLotAsync(Lot lot);
        Task UpdateLotAsync(Lot lot);
        Task DeleteLotAsync(int id);
        Task<IEnumerable<Lot>?> GetLotsBySiteIdAsync(int siteId);
        Task<IEnumerable<Lot>?> GetLotsByOrderIdAsync(int orderId);
    }
}
