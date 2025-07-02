namespace ITS.CoffeeMek.Models.QueueMessages
{
    public class MillingMachineDataQueueMessage
    {
        public int MachineId { get; set; }
        public int MachineStateId { get; set; }
        public double CycleTime { get; set; }
        public double Vibration { get; set; }
        public DateTime LocalTimeStamp { get; set; }
        public DateTime UTCTimeStamp { get; set; }
    }
}
