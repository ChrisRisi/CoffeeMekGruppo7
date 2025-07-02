using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services
{
    public class OrderFulfillmentService : IOrderFulfillmentService
    {
        private readonly ILotDataService _lotDataService;
        private readonly IOrderDataService _orderDataService;

        public OrderFulfillmentService(ILotDataService lotDataService, IOrderDataService orderDataService)
        {
            _lotDataService = lotDataService;
            _orderDataService = orderDataService;
        }

        public async Task ProcessOrderFulfillmentAsync()
        {
            var openOrders = await _orderDataService.GetOpenOrdersAsync();
            if (!openOrders.Any())
            {
                return;
            }

            var unassignedLots = (await _lotDataService.GetUnassignedLotsAsync()).OrderBy(lot => lot.StartTimeStamp).ToList();

            foreach (var order in openOrders)
            {
                var assignedLots = (await _lotDataService.GetLotsByOrderIdAsync(order.Id)).ToList();

                // First, check if the order is already fulfilled by the current quantity of its assigned lots.
                var totalCurrentQuantity = assignedLots.Sum(l => l.CurrentQuantity);
                if (totalCurrentQuantity >= order.Quantity)
                {
                    if (order.FulfillmentTime == null)
                    {
                        order.FulfillmentTime = DateTime.UtcNow;
                        await _orderDataService.UpdateOrderAsync(order);
                    }
                    continue; // This order is now fulfilled, move to the next one.
                }

                // If not fulfilled, check if we need to assign more lots based on capacity.
                var currentCapacity = assignedLots.Sum(l => l.Capacity);
                if (currentCapacity < order.Quantity && unassignedLots.Count != 0)
                {
                    // Assign available lots until the order's required capacity is met.
                    foreach (var lot in unassignedLots.ToList())
                    {
                        // Check if the lot's capacity can be added to the current order.
                        if (currentCapacity + lot.Capacity <= order.Quantity)
                        {
                            // Assign the lot to the order.
                            lot.OrderId = order.Id;
                            // The fulfillment check is inside UpdateLotAsync, so it will be triggered for each assigned lot.
                            await _lotDataService.UpdateLotAsync(lot);
                            currentCapacity += lot.Capacity;
                            unassignedLots.Remove(lot);
                            if (currentCapacity >= order.Quantity)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
