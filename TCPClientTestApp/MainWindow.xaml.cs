using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using Simulator;

namespace TCPMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		NetworkStream stream;
		BackgroundWorker bck;
		TcpClient client;

		public MainWindow()
		{
			InitializeComponent();            
        }



        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            tbDataReceived.Text = "Start Listening";
            bck = new BackgroundWorker();
            bck.WorkerReportsProgress = true;
            bck.ProgressChanged += Bck_ProgressChanged;
            bck.DoWork += Bck_DoWork;
            bck.RunWorkerAsync();
        }

        private void Bck_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpListener server = null;
            try
            {
                Int32 port = 3000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                //Start listening for client requests.
                server.Start();

                //Buffer for reading data
                Byte[] bytes = new Byte[8];

                //Enter the listening loop.
                while (true)
                {
                    bck.ReportProgress(0, "Waiting for a connection... ");

                    //Accept TcpClient
                    TcpClient client = server.AcceptTcpClient();
                    bck.ReportProgress(0, "Connected!");


                    stream = client.GetStream();

                    int i;

                    //Get all data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //display received data by reporting progress to the background worker
                        bck.ReportProgress(0, bytes[1].ToString() + ", " + bytes[6].ToString());
                    }

                    //Shutdown and end connection
                    client.Close();
                }

            }
            catch (Exception ex)
            {
                bck.ReportProgress(0, string.Format("SocketException: {0}", ex.ToString()));
            }
        }

        private void Bck_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string data = (string)e.UserState;

            tbDataReceived.Text = string.Format("Received: {0}", data) + Environment.NewLine + tbDataReceived.Text;
        }

        private void SendMessage(int Message)
        {
            //change IP address to the machine where you want to send the message to
            if(client==null)
            {
                client = new TcpClient("127.0.0.1", 2000);
            }
            
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = new byte[8];           
            bytesToSend[1] = (byte)Message;

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        /// <summary>
        /// /// eveniment folosit pentru a trimite o comanda catre proces/simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnSend_Click(object sender, RoutedEventArgs e)
		{	try
			{
                SendMessage((int)Command.LongDelay | (int)Command.PumpOne | (int)Command.PumpTwo | (int)Command.Started);             
			}
			catch (Exception ex)
			{
				tbDataReceived.Text = tbDataReceived + ex.ToString();
			}
		}

		

        /// <summary>
        /// eveniment folosit pentru a trimite o comanda catre proces/simulator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendMessage((int)Command.SmallDelay | (int)Command.PumpOne | (int)Command.PumpTwo | (int)Command.Stopped);               
            }
            catch (Exception ex)
            {
                tbDataReceived.Text = tbDataReceived + ex.ToString();
            }
        }
      
    }
}
