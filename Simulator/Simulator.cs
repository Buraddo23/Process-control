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
        Sensor_1 = 1,
        Sensor_2 = 2,
        Sensor_3 = 4,
        Sensor_4 = 8,
        Sensor_5 = 16
    }

    public class Simulator
    {
        private int _state;
        private int _waterLevel;
        private byte _outFlow1, _outFlow2;

        /// <summary>
        /// Initializes the process in the initial state
        /// </summary>
        /// <param name="pow1">Flowing power of Pump 1 (Flow/second). Recomended value: 2-12</param>
        /// <param name="pow2">Flowing power of Pump 2 (Flow/second). Recomended value: 2-12</param>
        public Simulator(byte pow1 = 6, byte pow2 = 6)
        {
            _state = 0;
            _waterLevel = 50;
            _outFlow1 = pow1;
            _outFlow2 = pow2;
        }

        /// <summary>
        /// Simulates the process for 1s
        /// </summary>
        /// <param name="CommandState">Input values for the process (see Command enum)</param>
        /// <param name="inf">Inflow rate set by potentiometer</param>
        /// <returns>State of the process (digital outputs) (see ProcessState enum)</returns>
        public byte[] UpdateState(int CommandState, byte inf)
        {
            //Determining Flow rate
            int flow = inf/16;
            Console.WriteLine(flow);
            if ((CommandState & (int)Command.PumpOne) != 0)
                flow -= _outFlow1;
            if ((CommandState & (int)Command.PumpTwo) != 0)
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

            //System.Console.Write("Water Level {0}\tProcess State {1}\n", _waterLevel, _state);

            Thread.Sleep(1000);
            return new byte[] { (byte)_state, (byte)_waterLevel };
        }
    }
}
