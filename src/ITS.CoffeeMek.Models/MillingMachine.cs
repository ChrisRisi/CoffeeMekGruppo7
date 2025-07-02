using System;

namespace ITS.CoffeeMek.Models
{
    public class MillingMachine : MachineTelemetry
    {
        public decimal? CycleTime { get; set; }
        public decimal? Vibration { get; set; }
    }
}