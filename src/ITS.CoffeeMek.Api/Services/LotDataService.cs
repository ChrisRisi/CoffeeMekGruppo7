using Dapper;
using ITS.CoffeeMek.Api.Services.Interfaces;
using ITS.CoffeeMek.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    /// <summary>
    /// Implementation of Lot data operations using Dapper and SQL Server
    /// </summary>
    public class LotDataService : ILotDataService
    {
        private readonly IDatabaseService _db;
        private readonly ILogger<LotDataService> _logger;
        private readonly IOrderDataService _orderDataService;

        public LotDataService(IDatabaseService db, ILogger<LotDataService> logger, IOrderDataService orderDataService)
        {
            _db = db;
            _logger = logger;
            _orderDataService = orderDataService;
        }

        public async Task<IEnumerable<Lot>> GetLotsAsync()
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT l.Id, l.OrderId, l.SiteId, l.Capacity, l.CurrentQuantity, 
                           l.StartTimeStamp, l.EndTimeStamp, l.PredictedStartTime, 
                           l.PredictedEndTime, l.Code,
                           o.Id as OId, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           s.Id as SId, s.Name, s.Country, s.Address,
                           c.Id as CId, c.Name, c.Address
                    FROM CoffeeMek.Lots l
                    LEFT JOIN CoffeeMek.Orders o ON l.OrderId = o.Id
                    LEFT JOIN CoffeeMek.Sites s ON l.SiteId = s.Id
                    LEFT JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    ORDER BY l.Id";

                var lotDict = new Dictionary<int, Lot>();
                
                await connection.QueryAsync<Lot, Order, Site, Client, Lot>(
                    query,
                    (lot, order, site, client) =>
                    {
                        if (!lotDict.TryGetValue(lot.Id, out var lotEntry))
                        {
                            lotEntry = lot;
                            lotDict.Add(lotEntry.Id, lotEntry);
                        }

                        if (order != null && lotEntry.Order == null) lotEntry.Order = order;
                        if (site != null && lotEntry.Site == null) lotEntry.Site = site;
                        if (client != null && lotEntry.Order != null && lotEntry.Order.Client == null) lotEntry.Order.Client = client;

                        return lotEntry;
                    },
                    splitOn: "OId,SId,CId"
                );
                
                return lotDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all lots");
                throw;
            }
        }

        public async Task<Lot?> GetLotByIdAsync(int id)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT l.Id, l.OrderId, l.SiteId, l.Capacity, l.CurrentQuantity, 
                           l.StartTimeStamp, l.EndTimeStamp, l.PredictedStartTime, 
                           l.PredictedEndTime, l.Code,
                           o.Id as OId, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           s.Id as SId, s.Name, s.Country, s.Address,
                           c.Id as CId, c.Name, c.Address
                    FROM CoffeeMek.Lots l
                    LEFT JOIN CoffeeMek.Orders o ON l.OrderId = o.Id
                    LEFT JOIN CoffeeMek.Sites s ON l.SiteId = s.Id
                    LEFT JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    WHERE l.Id = @Id";

                var lotDict = new Dictionary<int, Lot>();
                
                await connection.QueryAsync<Lot, Order, Site, Client, Lot>(
                    query,
                    (lot, order, site, client) =>
                    {
                        if (!lotDict.TryGetValue(lot.Id, out var lotEntry))
                        {
                            lotEntry = lot;
                            lotDict.Add(lotEntry.Id, lotEntry);
                        }

                        if (order != null && lotEntry.Order == null) lotEntry.Order = order;
                        if (site != null && lotEntry.Site == null) lotEntry.Site = site;
                        if (client != null && lotEntry.Order != null && lotEntry.Order.Client == null) lotEntry.Order.Client = client;

                        return lotEntry;
                    },
                    new { Id = id },
                    splitOn: "OId,SId,CId"
                );
                
                return lotDict.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving lot with ID {id}");
                throw;
            }
        }

        public async Task<int> InsertAsync(Lot lot)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    INSERT INTO CoffeeMek.Lots (
                        OrderId, SiteId, Capacity, CurrentQuantity, 
                        StartTimeStamp, EndTimeStamp, PredictedStartTime, 
                        PredictedEndTime, Code
                    )
                    VALUES (
                        @OrderId, @SiteId, @Capacity, @CurrentQuantity, 
                        @StartTimeStamp, @EndTimeStamp, @PredictedStartTime, 
                        @PredictedEndTime, @Code
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                
                return await connection.QuerySingleAsync<int>(query, lot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lot");
                throw;
            }
        }

        public async Task UpdateAsync(Lot lot)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    UPDATE CoffeeMek.Lots 
                    SET OrderId = @OrderId, 
                        SiteId = @SiteId, 
                        Capacity = @Capacity, 
                        CurrentQuantity = @CurrentQuantity, 
                        StartTimeStamp = @StartTimeStamp, 
                        EndTimeStamp = @EndTimeStamp, 
                        PredictedStartTime = @PredictedStartTime, 
                        PredictedEndTime = @PredictedEndTime, 
                        Code = @Code
                    WHERE Id = @Id";
                
                await connection.ExecuteAsync(query, lot);

                if (lot.OrderId.HasValue)
                {
                    await CheckOrderFulfillmentAsync(lot.OrderId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating lot with ID {lot.Id}");
                throw;
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
                await _orderDataService.UpdateAsync(order);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM CoffeeMek.Lots WHERE Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Lot>> GetAllLotsAsync()
        {
            using var connection = _db.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots";
            return await connection.QueryAsync<Lot>(query);
        }

        public async Task<IEnumerable<Lot>> GetLotsByOrderIdAsync(int orderId)
        {
            using var connection = _db.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots WHERE OrderId = @OrderId";
            return await connection.QueryAsync<Lot>(query, new { OrderId = orderId });
        }

        public async Task<IEnumerable<Lot>> GetUnassignedLotsAsync()
        {
            using var connection = _db.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Lots WHERE OrderId IS NULL";
            return await connection.QueryAsync<Lot>(query);
        }

        public async Task<IEnumerable<Lot>> GetLotsBySiteIdAsync(int siteId)
        {
            try
            {
                using var connection = _db.CreateConnection();
                const string query = @"
                    SELECT l.Id, l.OrderId, l.SiteId, l.Capacity, l.CurrentQuantity, 
                           l.StartTimeStamp, l.EndTimeStamp, l.PredictedStartTime, 
                           l.PredictedEndTime, l.Code,
                           o.Id as OId, o.ClientId, o.Quantity, o.CreationTime, o.FulfillmentTime,
                           s.Id as SId, s.Name, s.Country, s.Address,
                           c.Id as CId, c.Name, c.Address
                    FROM CoffeeMek.Lots l
                    LEFT JOIN CoffeeMek.Orders o ON l.OrderId = o.Id
                    LEFT JOIN CoffeeMek.Sites s ON l.SiteId = s.Id
                    LEFT JOIN CoffeeMek.Clients c ON o.ClientId = c.Id
                    WHERE l.SiteId = @SiteId
                    ORDER BY l.Id";

                var lotDict = new Dictionary<int, Lot>();
                
                await connection.QueryAsync<Lot, Order, Site, Client, Lot>(
                    query,
                    (lot, order, site, client) =>
                    {
                        if (!lotDict.TryGetValue(lot.Id, out var lotEntry))
                        {
                            lotEntry = lot;
                            lotDict.Add(lotEntry.Id, lotEntry);
                        }

                        if (order != null && lotEntry.Order == null) lotEntry.Order = order;
                        if (site != null && lotEntry.Site == null) lotEntry.Site = site;
                        if (client != null && lotEntry.Order != null && lotEntry.Order.Client == null) lotEntry.Order.Client = client;

                        return lotEntry;
                    },
                    new { SiteId = siteId },
                    splitOn: "OId,SId,CId"
                );
                
                return lotDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving lots for site ID {siteId}");
                throw;
            }
        }
    }
}