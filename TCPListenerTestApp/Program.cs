using System;
using System.ComponentModel;
using System.Threading;

namespace TCP_PLC
{
    public enum DigitalInputs0
    {
        Off         = 128,
        On          =  64,
        PumpOneTest =  16,
        PumpTwoTest =   8,
        Reset       =   4,
        M1Relay     =   2,
        M2Relay     =   1
    }

    public enum DigitalInputs1
    {
        Sensor_1 =  1,
        Sensor_2 =  2,
        Sensor_3 =  4,
        Sensor_4 =  8,
        Sensor_5 = 16
    }

    public enum DigitalOutputs
    {
        Alarm   = 128,
        OnLED   =  64,
        M1LED   =  16,
        M2LED   =   8,
        PumpOne =   2,
        PumpTwo =   1
    }

    /// <summary>
    /// Program  folosit  pentru a simula functionarea automatului.
    /// Exista o clasa Sender care trimite date catre apliatia de monitorizare
    /// si o clasa Listner care primeste comenzi de la aplicatia de monitorizare
    /// </summary>
    public class Program
	{
        /// <summary>
        /// To be called within a new thread to simulate a 0.5Hz signal to O0.3
        /// </summary>
        static void ClockGenerator()
        {
            while (true)
            {
                if (power)
                {
                    if ((plcIn[0] & (int)DigitalInputs0.M1Relay) != 0)
                    {
                        plcOut[0] ^= (int)DigitalOutputs.M1LED;
                    }
                    if ((plcIn[0] & (int)DigitalInputs0.M2Relay) != 0)
                    {
                        plcOut[0] ^= (int)DigitalOutputs.M2LED;
                    }
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// Test button function for pump 1
        /// </summary>
        static void PumpOneTest()
        {
            while (true)
            {
                if (power)
                {
                    if ((plcIn[0] & (int)DigitalInputs0.PumpOneTest) != 0)
                    {
                        plcOut[0] |= (int)DigitalOutputs.PumpOne
                                  | (int)DigitalOutputs.M1LED;
                        Thread.Sleep(3000);
                        plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpOne);
                        plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.M1LED);
                    }
                }
            }
        }

        /// <summary>
        /// Test button function for pump 2
        /// </summary>
        static void PumpTwoTest()
        {
            while (true)
            {
                if (power)
                {
                    if ((plcIn[0] & (int)DigitalInputs0.PumpTwoTest) != 0)
                    {
                        plcOut[0] |= (int)DigitalOutputs.PumpTwo
                                  | (int)DigitalOutputs.M2LED;
                        Thread.Sleep(3000);
                        plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpTwo);
                        plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.M2LED);
                    }
                }
            }
        }

        static Simulator.Simulator process;

        //I0.0 - OFF; I0.1 - ON; I0.2 - Unused; I0.3 - Test pump 1; I0.4 - Test pump 2; I0.5 - Reset; I0.6 - M1 emergency relay; I0.7 - M2 emergency relay
        //I1.0 - Unused; I1.1 - Unused; I1.2 - Unused; I1.3 - B5; I1.4 - B4; I1.5 - B3; I1.6 - B2; I1.7 - B1
        //I2 - Water level (8 bit res : 0-10V)
        //I3 - Y Inflow value from potentiometer (8 bit res)
        private static byte[] plcIn = new byte[4] { 0, 0, 0, 0 };

        public static void SetButtonState(byte inputs)
        {
            plcIn[0] = inputs;
        }

        public static void ModifyInflow(byte inf)
        {
            plcIn[3] = inf;
        }

        public static byte[] GetInputs()
        {
            return plcIn;
        }

        //O0.0 - Alarm; O0.1 - ON LED; O0.2 - Unused; O0.3 - M1 on LED; O0.4 - M2 on LED; O0.6 - M1; O0.7 - M2
        //O1 - Y Valve command (8 bit res : 0-16 units/sec)
        private static byte[] plcOut = new byte[2] { 0, 0 };

        public static byte[] GetOutputs()
        {
            return plcOut;
        }

        static bool power = false;
		static void Main(string[] args)
		{
            Console.WriteLine("Press enter to start simulation");
            Console.Read();

            process = new Simulator.Simulator();
            new Thread(ClockGenerator).Start();
            new Thread(PumpOneTest).Start();
            new Thread(PumpTwoTest).Start();
            bool brokenPump = false;

            new Thread(Send).Start();
            new Thread(Listen).Start();

            while (true)
            {
                //Power button functionality
                if ((plcIn[0] & (int)DigitalInputs0.On) != 0)
                {
                    power = true;
                    plcOut[0] |= (int)DigitalOutputs.OnLED;
                    plcOut[1] = plcIn[3];
                }
                if ((plcIn[0] & (int)DigitalInputs0.Off) != 0)
                {
                    power = false;
                    plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.OnLED);
                    plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpOne);
                    plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpTwo);
                    plcOut[1] = 0;
                }

                if (power)
                {
                    //Reset and Alarm behaviour
                    if ((plcIn[0] & (int)DigitalInputs0.Reset) != 0)
                    {
                        plcOut[0] = (int)DigitalOutputs.OnLED;
                        plcOut[1] = plcIn[3];
                        brokenPump = false;
                    }
                    if (brokenPump)
                    {
                        plcOut[0] = (int)DigitalOutputs.Alarm
                                  | (int)DigitalOutputs.OnLED;
                        plcOut[1] = 0;
                    }
                    else
                    {
                        //Emergency bad pump behaviour
                        if ((plcIn[0] & (int)DigitalInputs0.M1Relay) != 0)
                        {
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpOne);
                            plcOut[0] |= (int)DigitalOutputs.Alarm;
                            brokenPump = true;
                        }
                        if ((plcIn[0] & (int)DigitalInputs0.M2Relay) != 0)
                        {
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpTwo);
                            plcOut[0] |= (int)DigitalOutputs.Alarm;
                            brokenPump = true;
                        }

                        //Pump RS flip-flop based on level reported by sensors
                        if ((plcIn[1] & (int)DigitalInputs1.Sensor_3) != 0)
                        {
                            plcOut[0] |= (int)DigitalOutputs.PumpOne
                                      | (int)DigitalOutputs.PumpTwo
                                      | (int)DigitalOutputs.M1LED
                                      | (int)DigitalOutputs.M2LED
                                      | (int)DigitalOutputs.Alarm;
                        }
                        if ((plcIn[1] & ((int)DigitalInputs1.Sensor_1 | (int)DigitalInputs1.Sensor_4)) == 0)
                        {
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpOne);
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.PumpTwo);
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.M1LED);
                            plcOut[0] = (byte)(plcOut[0] & ~(int)DigitalOutputs.M2LED);
                        }
                        if ((plcIn[1] & (int)DigitalInputs1.Sensor_2) != 0)
                        {
                            plcOut[0] |= (int)DigitalOutputs.PumpOne
                                      | (int)DigitalOutputs.M1LED;
                        }
                        if ((plcIn[1] & (int)DigitalInputs1.Sensor_5) != 0)
                        {
                            plcOut[0] |= (int)DigitalOutputs.PumpTwo
                                      | (int)DigitalOutputs.M2LED;
                        }
                    }
                    
                    //Prepare the output format
                    int command = 0;
                    if ((plcOut[0] & (int)DigitalOutputs.PumpOne) != 0)
                    {
                        command |= (int)Simulator.Command.PumpOne;
                    }
                    if ((plcOut[0] & (int)DigitalOutputs.PumpTwo) != 0)
                    {
                        command |= (int)Simulator.Command.PumpTwo;
                    }

                    //Set the outputs of the PLC
                    process.SetCommand(command, plcOut[1]);
                                 
                }
                //Update the inputs of the PLC
                plcIn[1] = process.GetState()[0];
                plcIn[2] = process.GetState()[1];
                //Console.WriteLine("I0: {0}\tI1: {1}\tI2: {2}\nO0: {3}\tO1: {4}\n", plcIn[0], plcIn[1], plcIn[2], plcOut[0], plcOut[1]);
            }
		}
        
		private static void Send()
        {
            Sender sender = new Sender();
            sender.Send();
        }

        private static void Listen()
        {
            Listener listener = new Listener();
            listener.Listen();
        }
    }
}
