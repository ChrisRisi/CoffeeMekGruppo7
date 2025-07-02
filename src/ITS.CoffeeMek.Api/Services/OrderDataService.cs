using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    /// <summary>
    /// Implementation of Order data operations using Dapper and SQL Server
    /// </summary>
    public class OrderDataService : IOrderDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<OrderDataService> _logger;

        public OrderDataService(IDatabaseService db, ILogger<OrderDataService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT o.Id, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           c.Id as Id, c.Name, c.Address
                    FROM CoffeeMek.Orders o
                    JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    ORDER BY o.Id";

                var orderDict = new Dictionary<int, Order>();
                
                await connection.QueryAsync<Order, Client, Order>(
                    query,
                    (order, client) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Client = client;
                            orderDict.Add(orderEntry.Id, orderEntry);
                        }
                        return orderEntry;
                    },
                    splitOn: "Id"
                );
                
                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT o.Id, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           c.Id as ClientId, c.Name, c.Address
                    FROM CoffeeMek.Orders o
                    JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    WHERE o.FulfillmentTime IS NULL
                    ORDER BY o.Id";

                var orderDict = new Dictionary<int, Order>();

                await connection.QueryAsync<Order, Client, Order>(
                    query,
                    (order, client) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Client = client;
                            orderDict.Add(orderEntry.Id, orderEntry);
                        }
                        return orderEntry;
                    },
                    splitOn: "ClientId"
                );

                return orderDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving open orders");
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT o.Id, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           c.Id as ClientId, c.Name, c.Address
                    FROM CoffeeMek.Orders o
                    JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    WHERE o.Id = @Id";

                var orderDict = new Dictionary<int, Order>();
                
                await connection.QueryAsync<Order, Client, Order>(
                    query,
                    (order, client) =>
                    {
                        if (!orderDict.TryGetValue(order.Id, out var orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Client = client;
                            orderDict.Add(orderEntry.Id, orderEntry);
                        }
                        return orderEntry;
                    },
                    new { Id = id },
                    splitOn: "ClientId"
                );
                
                return orderDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(Order order)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Orders (ClientId, Quantity, CreationTime, FulfillmentTime)
                    VALUES (@ClientId, @Quantity, @CreationTime, @FulfillmentTime);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                // Set creation time to UTC now if not provided
                if (order.CreationTime == default)
                {
                    order.CreationTime = DateTime.UtcNow;
                }
                
                return await connection.QuerySingleAsync<int>(query, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task UpdateAsync(Order order)
        {
            try
            {
                // Get the existing order to preserve creation time
                Order? existingOrder = null;

                using (var connection = _db.CreateConnection())
                {
                    existingOrder = await connection.QueryFirstOrDefaultAsync<Order>(
                        "SELECT CreationTime FROM CoffeeMek.Orders WHERE Id = @Id",
                        new { Id = order.Id });
                }
                
                
                if (existingOrder != null)
                {
                    order.CreationTime = existingOrder.CreationTime;
                }
                
                using (var connection = _db.CreateConnection())
                {
                    const string query = @"
                    UPDATE CoffeeMek.Orders 
                    SET ClientId = @ClientId, 
                        Quantity = @Quantity, 
                        FulfillmentTime = @FulfillmentTime
                    WHERE Id = @Id";

                    await connection.ExecuteAsync(query, order);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with ID {order.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"DELETE FROM CoffeeMek.Orders WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {id}");
                throw;
            }
        }
    }
}