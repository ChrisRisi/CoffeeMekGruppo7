using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces
{
    public interface IOrderDataService
    {
        Task<IEnumerable<Order>> GetOpenOrdersAsync();
        Task UpdateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
