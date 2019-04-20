using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_PLC
{
    /// <summary>
    /// Program  folosit  pentru a simula functionarea automatului.
    /// Exista o clasa Sender care trimite date catre apliatia de monitorizare
    /// si o clasa Listner care primeste comenzi de la aplicatia de monitorizare
    /// partea de comunicare cu aplicatia de monitorizare trebuie implementata in paralel
    /// existand un thread separat pentru transmiterea de date si un thread separat pentru receptionarea de date
    /// </summary>
    public class Program
	{        
        static BackgroundWorker senderWorker;	
        static Simulator.Simulator process;

		static void Main(string[] args)
		{
            Console.WriteLine("Press enter to start...");
            Console.ReadLine();

            process = new Simulator.Simulator();


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
            listenThread.Start();
		}

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
        }
	}
}
