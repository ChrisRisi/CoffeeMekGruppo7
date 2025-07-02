using System;

namespace ITS.CoffeeMek.Models
{
    public class Lathe : MachineTelemetry
    {
        public decimal? RotationSpeed { get; set; }
        public decimal? SpindleTemperature { get; set; }
    }
}