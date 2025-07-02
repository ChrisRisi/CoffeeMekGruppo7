using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services
{
    public class OrderDataService : IOrderDataService
    {
        private readonly DapperContext _context;

        public OrderDataService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync()
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Orders WHERE FulfillmentTime IS NULL";
            return await connection.QueryAsync<Order>(query);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            using var connection = _context.CreateConnection();
            const string query = "UPDATE CoffeeMek.Orders SET FulfillmentTime = @FulfillmentTime WHERE Id = @Id";
            await connection.ExecuteAsync(query, order);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Orders WHERE Id = @OrderId";
            return await connection.QuerySingleOrDefaultAsync<Order>(query, new { OrderId = orderId });
        }
    }
}
