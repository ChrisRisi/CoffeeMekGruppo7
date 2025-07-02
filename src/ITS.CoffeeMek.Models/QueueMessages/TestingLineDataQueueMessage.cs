namespace ITS.CoffeeMek.Models.QueueMessages
{
    public class TestingLineDataQueueMessage
    {
        public int MachineId { get; set; }
        public int MachineStateId { get; set; }
        public double BoilerPressure { get; set; }
        public double BoilerTemperature { get; set; }
        public double EnergyConsumption { get; set; }
        public DateTime LocalTimeStamp { get; set; }
        public DateTime UTCTimeStamp { get; set; }
    }
}
