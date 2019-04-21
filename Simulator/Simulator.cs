using System;
using System.Threading;

namespace Simulator
{

    /// <summary>
    /// Enum used for transmission of commands
    /// </summary>
    public enum Command
    {       
        PumpOne = 1,
        PumpTwo = 2
    }

    /// <summary>
    /// Enum used to represent process state
    /// </summary>
    public enum ProcessState
    {
        Sensor_1 =  1,
        Sensor_2 =  2,
        Sensor_3 =  4,
        Sensor_4 =  8,
        Sensor_5 = 16
    }

    public class Simulator
    {
        private int _state;
        private int _waterLevel;
        private byte _outFlow1, _outFlow2;

        private int _commandState;
        private byte _inf;
        public void SetCommand(int com, byte inf)
        {
            _commandState = com;
            _inf = inf;
        }
        public byte[] GetState()
        {
            return new byte[] { (byte)_state, (byte)_waterLevel };
        }
        

        /// <summary>
        /// Initializes the process in the initial state
        /// </summary>
        /// <param name="pow1">Flowing power of Pump 1 (Flow/second). Recomended value: 2-12</param>
        /// <param name="pow2">Flowing power of Pump 2 (Flow/second). Recomended value: 2-12</param>
        /// <param name="wl">Initial water level in tank (0-255)</param>
        public Simulator(byte pow1 = 6, byte pow2 = 6, byte wl = 50)
        {
            _state = 0;
            _waterLevel = wl;
            _outFlow1 = pow1;
            _outFlow2 = pow2;

            new Thread(UpdateState).Start();
        }

        /// <summary>
        /// Simulates the process for 1s
        /// </summary>
        /// <param name="CommandState">Input values for the process (see Command enum)</param>
        /// <param name="inf">Inflow rate set by potentiometer</param>
        /// <returns>State of the process (digital outputs) (see ProcessState enum)</returns>
        public void UpdateState()
        {
            while (true)
            {
                //Determining Flow rate
                int flow = _inf / 16;
                if ((_commandState & (int)Command.PumpOne) != 0)
                    flow -= _outFlow1;
                if ((_commandState & (int)Command.PumpTwo) != 0)
                    flow -= _outFlow2;

                //Applying flow rate
                _waterLevel += flow;
                if (_waterLevel >= 255)
                    _waterLevel = 255;
                if (_waterLevel <= 0)
                    _waterLevel = 0;

                //Creating the state of the sensors
                if (_waterLevel >= 35) _state |= (int)ProcessState.Sensor_4 | (int)ProcessState.Sensor_1;
                else _state &= ~((int)ProcessState.Sensor_4 | (int)ProcessState.Sensor_1);

                if (_waterLevel >= 145) _state |= (int)ProcessState.Sensor_2;
                else _state &= ~(int)ProcessState.Sensor_2;

                if (_waterLevel >= 185) _state |= (int)ProcessState.Sensor_5;
                else _state &= ~(int)ProcessState.Sensor_5;

                if (_waterLevel >= 235) _state |= (int)ProcessState.Sensor_3;
                else _state &= ~(int)ProcessState.Sensor_3;

                Thread.Sleep(1000);
            }
        }
    }
}
