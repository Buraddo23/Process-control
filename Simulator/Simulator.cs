using System.Threading;

namespace Simulator
{

    /// <summary>
    /// Enumerare folosita pentru a transmite/receptiona comenzi
    /// </summary>
    public enum Command
    {       
        PumpOne = 1,
        PumpTwo = 2
    }

    /// <summary>
    /// Enumerare folosita pentru a modela starea procesului
    /// </summary>
    public enum ProcessState
    {        
        PumpOne = 1,
        PumpTwo = 2,
        Sensor_1 = 4,
        Sensor_2 = 8,
        Sensor_3 = 16,
        Sensor_4 = 32,
        Sensor_5 = 64,
    }

    public class Simulator
    {
        private int _state;
        private int _waterLevel;
        private int _inFlow, _outFlow1, _outFlow2;

        public Simulator(int inf = 8, int out1 = 6, int out2 = 6)
        {
            _state = 0;
            _waterLevel = 0;
            _inFlow = inf;
            _outFlow1 = out1;
            _outFlow2 = out2;
        }

        public void UpdateState(byte CommandState)
        {
            //Determining Flow rate
            int flow = _inFlow;
            if ((CommandState | (int)Command.PumpOne) != 0)
            {
                flow -= _outFlow1;
                _state |= (int)ProcessState.PumpOne;
            }
            else
            {
                _state &= ~(int)ProcessState.PumpOne;
            }
            if ((CommandState | (int)Command.PumpTwo) != 0)
            {
                flow -= _outFlow2;
                _state |= (int)ProcessState.PumpTwo;
            }
            else
            {
                _state &= ~(int)ProcessState.PumpTwo;
            }

            //Applying flow rate
            _waterLevel += flow;
            if (_waterLevel >= 255)
            {
                _waterLevel = 255;
                System.Console.WriteLine("Tank overflow");
            }

            //Creating the state of the sensors
            if (_waterLevel >= 35) _state |= (int)ProcessState.Sensor_4 | (int)ProcessState.Sensor_1;
            else _state &= ~((int)ProcessState.Sensor_4 | (int)ProcessState.Sensor_1);

            if (_waterLevel >= 145) _state |= (int)ProcessState.Sensor_2;
            else _state &= ~(int)ProcessState.Sensor_2;

            if (_waterLevel >= 185) _state |= (int)ProcessState.Sensor_5;
            else _state &= ~(int)ProcessState.Sensor_5;

            if (_waterLevel >= 235) _state |= (int)ProcessState.Sensor_3;
            else _state &= ~(int)ProcessState.Sensor_3;

            System.Console.WriteLine(_state);

            Thread.Sleep(1000);
        }
        public int GetState()
        {
            return _state;
        }

        /*private void ExecuteLongDelayCommand(int Delay)
        {
            System.Threading.Thread.Sleep(Delay);
            _state[0] = (int)ProcessState.PumpOne;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpOne 
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4 
                | (int)ProcessState.Level_5;
            System.Threading.Thread.Sleep(Delay);
        }

        private void ExecuteShortDelayCommand(int Delay)
        {
            _state[0] = (int)ProcessState.PumpOne
                | (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4 
                | (int)ProcessState.Level_5;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.PumpTwo 
                | (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4 
                | (int)ProcessState.Level_5;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4 
                | (int)ProcessState.Level_5;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3 
                | (int)ProcessState.Level_4;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1 
                | (int)ProcessState.Level_2 
                | (int)ProcessState.Level_3;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1
                | (int)ProcessState.Level_2;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Level_1;
            System.Threading.Thread.Sleep(Delay);

            _state[0] = (int)ProcessState.Stopped;
            System.Threading.Thread.Sleep(Delay);
        }
        

        public void UpdateState(int CommandState)
        {            
            int longDelayForBothPump = (int)Command.LongDelay 
                | (int)Command.PumpOne 
                | (int)Command.PumpTwo 
                | (int)Command.Started;

            int shortDelayForBothPump = (int)Command.SmallDelay 
                | (int)Command.PumpOne 
                | (int)Command.PumpTwo 
                | (int)Command.Stopped;

            if(CommandState == longDelayForBothPump)
            {
                ExecuteLongDelayCommand(5000);
            }
            else if (CommandState == shortDelayForBothPump)
            {
                ExecuteShortDelayCommand(2000);
            }            
        }*/
    }
}
