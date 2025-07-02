using System;

namespace ITS.CoffeeMek.Models
{
    public class TestingLine : MachineTelemetry
    {
        public decimal? BoilerPressure { get; set; }
        public decimal? BoilerTemperature { get; set; }
        public decimal? EnergyConsumption { get; set; }
    }
}