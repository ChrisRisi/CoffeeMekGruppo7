using System;
using System.Collections.Generic;

namespace ITS.CoffeeMek.Models
{
    public class Lot
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public int? SiteId { get; set; }
        public Site? Site { get; set; }
        public int Capacity { get; set; }
        public int CurrentQuantity { get; set; }
        public DateTime? StartTimeStamp { get; set; }
        public DateTime? EndTimeStamp { get; set; }
        public DateTime? PredictedStartTime { get; set; }
        public DateTime? PredictedEndTime { get; set; }
        public string? Code { get; set; }
    }
}