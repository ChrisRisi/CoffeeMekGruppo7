using System;
using System.Collections.Generic;

namespace ITS.CoffeeMek.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime? FulfillmentTime { get; set; }

        public Client Client { get; set; } = null!;
    }
}
