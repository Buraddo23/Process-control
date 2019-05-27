namespace PLCWebControl.Models
{
    public class StateDataModel
    {
        public byte Buttons { private get; set; }
        public byte Sensors { private get; set; }
        public byte WaterLevel { get; set; }
        public byte Inflow { get; set; }
        public byte Outputs { private get; set; }
        public bool OffButtonState
        {
            get { return (Buttons & 128) != 0; }
        }
        public bool OnButtonState
        {
            get { return (Buttons & 64) != 0; }
        }
        public bool Pump1ButtonState
        {
            get { return (Buttons & 16) != 0; }
        }
        public bool Pump2ButtonState
        {
            get { return (Buttons & 8) != 0; }
        }
        public bool ResetButtonState
        {
            get { return (Buttons & 4) != 0; }
        }
        public bool M1RelayButtonState
        {
            get { return (Buttons & 2) != 0; }
        }
        public bool M2RelayButtonState
        {
            get { return (Buttons & 1) != 0; }
        }
        public bool[] SensorState
        {
            get { return new bool[] { (Sensors & 1) != 0, (Sensors & 2) != 0, (Sensors & 4) != 0, (Sensors & 8) != 0, (Sensors & 16) != 0 }; }
        }
        public bool Alarm
        {
            get { return (Outputs & 128) != 0; }
        }
        public bool ONLED
        {
            get { return (Outputs & 64) != 0; }
        }
        public bool M1ONLED
        {
            get { return (Outputs & 16) != 0; }
        }
        public bool M2ONLED
        {
            get { return (Outputs & 8) != 0; }
        }
        public bool PumpOne
        {
            get { return (Outputs & 2) != 0; }
        }
        public bool PumpTwo
        {
            get { return (Outputs & 1) != 0; }
        }
    }
}