using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services
{
    public class LotDataService : ILotDataService
    {
        private readonly DapperContext _context;
        private readonly IOrderDataService _orderDataService;

        public LotDataService(DapperContext context, IOrderDataService orderDataService)
        {
            _context = context;
            _orderDataService = orderDataService;
        }

        public async Task<Lot?> GetActiveLotBySiteIdAsync(int siteId)
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots WHERE SiteId = @siteId AND EndTimeStamp IS NULL";
            var lots = await connection.QueryAsync<Lot>(query, new { siteId });
            return lots.FirstOrDefault();
        }

        public async Task UpdateLotAsync(Lot lot)
        {
            using var connection = _context.CreateConnection();
            const string query = @"
                UPDATE CoffeeMek.Lots 
                SET OrderId = @OrderId, 
                    SiteId = @SiteId, 
                    Capacity = @Capacity, 
                    CurrentQuantity = @CurrentQuantity, 
                    StartTimeStamp = @StartTimeStamp, 
                    EndTimeStamp = @EndTimeStamp, 
                    Code = @Code
                WHERE Id = @Id";
            await connection.ExecuteAsync(query, lot);

            if (lot.OrderId.HasValue)
            {
                await CheckOrderFulfillmentAsync(lot.OrderId.Value);
            }
        }

        private async Task CheckOrderFulfillmentAsync(int orderId)
        {
            var order = await _orderDataService.GetOrderByIdAsync(orderId);
            if (order == null || order.FulfillmentTime.HasValue)
            {
                return;
            }

            var assignedLots = await GetLotsByOrderIdAsync(orderId);
            if (assignedLots.Sum(l => l.CurrentQuantity) >= order.Quantity)
            {
                order.FulfillmentTime = DateTime.UtcNow;
                await _orderDataService.UpdateOrderAsync(order);
            }
        }

        public async Task<Lot> CreateLotAsync(Lot lot)
        {
            using var connection = _context.CreateConnection();
            const string query = @"
                INSERT INTO CoffeeMek.Lots (OrderId, SiteId, Capacity, CurrentQuantity, StartTimeStamp, Code)
                VALUES (@OrderId, @SiteId, @Capacity, @CurrentQuantity, @StartTimeStamp, @Code);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            var id = await connection.QuerySingleAsync<int>(query, lot);
            lot.Id = id;
            return lot;
        }

        public async Task<IEnumerable<Lot>> GetAllLotsAsync()
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots";
            return await connection.QueryAsync<Lot>(query);
        }

        public async Task<IEnumerable<Lot>> GetLotsByOrderIdAsync(int orderId)
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots WHERE OrderId = @OrderId";
            return await connection.QueryAsync<Lot>(query, new { OrderId = orderId });
        }

        public async Task<IEnumerable<Lot>> GetUnassignedLotsAsync()
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots WHERE OrderId IS NULL";
            return await connection.QueryAsync<Lot>(query);
        }
    }
}
