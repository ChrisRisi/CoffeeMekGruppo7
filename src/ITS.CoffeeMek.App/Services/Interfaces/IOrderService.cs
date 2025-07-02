using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>?> GetOrdersAsync();
        Task<IEnumerable<Order>?> GetOpenOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
    }
}
