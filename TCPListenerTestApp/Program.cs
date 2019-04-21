using System;
using System.ComponentModel;
using System.Threading;

namespace TCP_PLC
{
    public enum digitalInputs0
    {
        off       = 128,
        on        =  64,
        pump1Test =  16,
        pump2Test =   8,
        reset     =   4,
        M1Relay   =   2,
        M2Relay   =   1
    }

    public enum digitalInputs1
    {
        Sensor_1 =  1,
        Sensor_2 =  2,
        Sensor_3 =  4,
        Sensor_4 =  8,
        Sensor_5 = 16
    }

    public enum digitalOutputs
    {
        alarm   = 128,
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
        static void clockGenerator()
        {
            if ((PLCin[0] | (int)digitalInputs0.M1Relay) != 0)
            {
                PLCout[0] ^= (int)digitalOutputs.M1LED;
            }
            if ((PLCin[0] | (int)digitalInputs0.M2Relay) != 0)
            {
                PLCout[0] ^= (int)digitalOutputs.M2LED;
            }
            Thread.Sleep(500);
        }

        //static BackgroundWorker senderWorker;	
        static Simulator.Simulator process;

        //I0.0 - OFF; I0.1 - ON; I0.2 - Unused; I0.3 - Test pump 1; I0.4 - Test pump 2; I0.5 - Reset; I0.6 - M1 emergency relay; I0.7 - M2 emergency relay
        //I1.0 - Unused; I1.1 - Unused; I1.2 - Unused; I1.3 - B5; I1.4 - B4; I1.5 - B3; I1.6 - B2; I1.7 - B1
        //I2 - Water level (8 bit res : 0-10V)
        static byte[] PLCin = new byte[3] { 0, 0, 0 };

        //O0.0 - Alarm; O0.1 - ON LED; O0.2 - Unused; O0.3 - M1 on LED; O0.4 - M2 on LED; O0.6 - M1; O0.7 - M2
        //O1 - Y Inflow (8 bit res : 0-16 units/sec)
        static byte[] PLCout = new byte[2] { 0, 0 };

		static void Main(string[] args)
		{
            process = new Simulator.Simulator();
            Thread clockGeneratorThread = new Thread(clockGenerator);
            clockGeneratorThread.Start();

            bool power = false;

            while (true)
            {
                //Power button functionality
                if ((PLCin[0] | (int)digitalInputs0.on) != 0)
                {
                    power = true;
                    PLCout[0] |= (int)digitalOutputs.OnLED;
                }
                if ((PLCin[0] | (int)digitalInputs0.off) != 0)
                {
                    power = false;
                    PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.OnLED);
                }
                
                if (power)
                {
                    //Pump RS flip-flop based on level reported by sensors
                    if ((PLCin[1] | (int)digitalInputs1.Sensor_1 | (int)digitalInputs1.Sensor_4) == 0)
                    {
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.PumpOne);
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.PumpTwo);
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.M1LED);
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.M2LED);
                    }
                    if ((PLCin[1] | (int)digitalInputs1.Sensor_2) != 0)
                    {
                        PLCout[0] |= (int)digitalOutputs.PumpOne;
                        PLCout[0] |= (int)digitalOutputs.M1LED;
                    }
                    if ((PLCin[1] | (int)digitalInputs1.Sensor_5) != 0)
                    {
                        PLCout[0] |= (int)digitalOutputs.PumpTwo;
                        PLCout[0] |= (int)digitalOutputs.M2LED;
                    }

                    //Emergency bad pump behaviour
                    if ((PLCin[0] | (int)digitalInputs0.M1Relay) != 0)
                    {
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.PumpOne);
                    }
                    if ((PLCin[0] | (int)digitalInputs0.M2Relay) != 0)
                    {
                        PLCout[0] = (byte)(PLCout[0] & ~(int)digitalOutputs.PumpTwo);
                    }             
                    
                    //Prepare the output format
                    byte[] processOutputs;
                    int command = 0;
                    if ((PLCout[0] & (int)digitalOutputs.PumpOne) != 0)
                    {
                        command |= (int)Simulator.Command.PumpOne;
                    }
                    if ((PLCout[0] & (int)digitalOutputs.PumpTwo) != 0)
                    {
                        command |= (int)Simulator.Command.PumpTwo;
                    }
                                       
                    //Update the inputs and the outputs of the PLC
                    processOutputs = process.UpdateState(command, PLCout[1]);
                    PLCin[1] = processOutputs[0];
                    PLCin[2] = processOutputs[1];

                    Console.WriteLine("I0: {0}\tI1: {1}\tI2: {2}\t\tO0: {3}\tO1: {4}\n", PLCin[0], PLCin[1], PLCin[2], PLCout[0], PLCout[1]);
                }
            }

            /*
            ///pentru partea de transmitere de date am folosit un background worker 
            ///care poate sa ruleze o metoda intr-un thread separat (metoda rulata este SenderWorker_DoWork)
            ///atentie la partea de update a UI-ului, pentru a face update la UI din background worker se foloseste
            ///SenderWorker_ProgressChanged, daca nu folosit aceasta metoda se poate sa primiti exceptii legate de
            ///faptul ca incercati sa faceti update pe UI dintr-un alt thread decat cel principal
            senderWorker = new BackgroundWorker();
			senderWorker.WorkerReportsProgress = true;
			senderWorker.DoWork += SenderWorker_DoWork;
			senderWorker.ProgressChanged += SenderWorker_ProgressChanged;
			senderWorker.RunWorkerAsync();

            /// varianta mai noua de implementare a paralelismului utilizand TPL
            /// este recomandat sa utilizati aceasta varianta, este mai usor de gestionat si ofera mai multe facilitati
            /// mai multe informatii si exemple: https://www.codeproject.com/Articles/1083787/Tasks-and-Task-Parallel-Library-TPL-Multi-threadin
            Thread listenThread = new Thread(Listen);
            listenThread.Start();*/
		}
        /*
		private static void SenderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Console.WriteLine(e.UserState);
		}
				
		private static void SenderWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Sender dataSender = new Sender(senderWorker, process);
			dataSender.Send();
		}

        private static void Listen()
        {
            Listener listener = new Listener(process);
            listener.Listen();
        }*/
	}
}
