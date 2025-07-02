using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for Order data operations
    /// </summary>
    public interface IOrderDataService
    {
        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns>Collection of orders</returns>
        Task<IEnumerable<Order>> GetOrdersAsync();

        /// <summary>
        /// Gets all open orders (orders that have not been fulfilled yet).
        /// </summary>
        /// <returns>Collection of open orders</returns>
        Task<IEnumerable<Order>> GetOpenOrdersAsync();

        
        /// <summary>
        /// Gets an order by its ID
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>The order if found, null otherwise</returns>
        Task<Order?> GetOrderByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new order
        /// </summary>
        /// <param name="order">The order to insert</param>
        /// <returns>The ID of the newly created order</returns>
        Task<int> InsertAsync(Order order);
        
        /// <summary>
        /// Updates an existing order
        /// </summary>
        /// <param name="order">The order with updated values</param>
        Task UpdateAsync(Order order);
        
        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="id">The ID of the order to delete</param>
        Task DeleteAsync(int id);
    }
}