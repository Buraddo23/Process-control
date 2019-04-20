using System;
using System.ComponentModel;
using System.Threading;

namespace TCP_PLC
{
    /// <summary>
    /// Program  folosit  pentru a simula functionarea automatului.
    /// Exista o clasa Sender care trimite date catre apliatia de monitorizare
    /// si o clasa Listner care primeste comenzi de la aplicatia de monitorizare
    /// </summary>
    public class Program
	{        
        //static BackgroundWorker senderWorker;	
        static Simulator.Simulator process;
        static byte[] PLCin = new byte[3] { 0, 0, 0 };
        static byte[] PLCout = new byte[2] { 3, 128 };


		static void Main(string[] args)
		{
            Console.WriteLine("Press enter to start...");
            Console.ReadLine();

            process = new Simulator.Simulator();

            while (true)
            {
                byte[] processOutputs;

                int command = 0;
                if ((PLCout[0] & 0x02) != 0)
                    command |= (int)Simulator.Command.PumpOne;
                if ((PLCout[0] & 0x01) != 0)
                    command |= (int)Simulator.Command.PumpTwo;

                processOutputs = process.UpdateState(command, PLCout[1]);
                //PLCin[0] = 
                PLCin[1] = processOutputs[0];
                PLCin[2] = processOutputs[1];

                Console.WriteLine("I0: {0}\tI1: {1}\tI2: {2}\t\tO0: {3}\tO1: {4}\n", PLCin[0], PLCin[1], PLCin[2], PLCout[0], PLCout[1]);
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
