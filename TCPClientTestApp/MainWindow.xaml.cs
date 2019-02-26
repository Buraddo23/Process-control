using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCPClientTestApp
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

		private void Listen()
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
				String data = null;


				//Enter the listening loop.
				while (true)
				{
					bck.ReportProgress(0, "Waiting for a connection... ");

					//Accept TcpClient
					TcpClient client = server.AcceptTcpClient();
					bck.ReportProgress(0, "Connected!");
					

					data = null;

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

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{	try
			{	
				if (client == null)
					CreateConnection();
				
				NetworkStream nwStream = client.GetStream();
				byte[] bytesToSend = new byte[8];
				bytesToSend[1] = 1;
				bytesToSend[6] = 2;
								
				nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			}
			catch (Exception ex)
			{
				tbDataReceived.Text = tbDataReceived + ex.ToString();
			}
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
			Listen();
		}

		private void Bck_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			string data = (string)e.UserState;
			
			tbDataReceived.Text = tbDataReceived.Text + Environment.NewLine + string.Format("Received: {0}", data);			
		}

		private void CreateConnection()
		{
			//change IP address to the machine where you want to send the message to
			client = new TcpClient("127.0.0.1", 2000);
		}
	}
}
