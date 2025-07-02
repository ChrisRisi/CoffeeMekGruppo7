using System;

namespace ITS.CoffeeMek.Models
{
    public class AssemblyLine : MachineTelemetry
    {
        public decimal? AvgStationTime { get; set; }
        public int? OperatorCount { get; set; }
    }
}