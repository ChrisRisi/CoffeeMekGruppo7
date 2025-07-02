using System;

namespace ITS.CoffeeMek.Models
{
    public abstract class MachineTelemetry
    {
        public int Id { get; set; }
        public int MachineId { get; set; }
        public int? MachineStateId { get; set; }
        public DateTime? LocalTimeStamp { get; set; }
        public DateTime? UTCTimeStamp { get; set; }
        public DateTime? LastMaintenance { get; set; }
        
        public Machine Machine { get; set; } = null!;
        public MachineState? MachineState { get; set; }
    }
}