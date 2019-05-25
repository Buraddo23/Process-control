namespace PLCWebControl.Models
{
    public class StateDataModel
    {
        public byte Buttons { get; set; }
        public byte Sensors { get; set; }
        public byte WaterLevel { get; set; }
        public byte Inflow { get; set; }
    }
}