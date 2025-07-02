namespace ITS.CoffeeMek.Models.QueueMessages
{
    public class AssemblyLineDataQueueMessage
    {
        public int MachineId { get; set; }
        public int MachineStateId { get; set; }
        public double AvgStationTime { get; set; }
        public int OperatorCount { get; set; }
        public DateTime LocalTimeStamp { get; set; }
        public DateTime UTCTimeStamp { get; set; }
    }
}
