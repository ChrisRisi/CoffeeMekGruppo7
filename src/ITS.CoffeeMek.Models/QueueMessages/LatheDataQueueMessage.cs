namespace ITS.CoffeeMek.Models.QueueMessages
{
    public class LatheDataQueueMessage
    {
        public int MachineId { get; set; }
        public int MachineStateId { get; set; }
        public double RotationSpeed { get; set; }
        public double SpindleTemperature { get; set; }
        public DateTime LocalTimeStamp { get; set; }
        public DateTime UTCTimeStamp { get; set; }
    }
}
