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
				// Set the TcpListener on port 13000.
				Int32 port = 2000;
				IPAddress localAddr = IPAddress.Parse("10.120.8.100");

				// TcpListener server = new TcpListener(port);
				server = new TcpListener(localAddr, port);

				// Start listening for client requests.
				server.Start();

				// Buffer for reading data
				Byte[] bytes = new Byte[8];
				String data = null;


				//Enter the listening loop.
				while (true)
				{
					bck.ReportProgress(0, "Waiting for a connection... ");					

					// Perform a blocking call to accept requests.
					// You could also user server.AcceptSocket() here.
					TcpClient client = server.AcceptTcpClient();
					bck.ReportProgress(0, "Connected!");
					

					data = null;

					stream = client.GetStream();

					int i;

					// Loop to receive all the data sent by the client.
					while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						// Translate data bytes to a ASCII string.
						data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
						bck.ReportProgress(0, bytes[1].ToString() + ", " + bytes[6].ToString());					
					}
					// Shutdown and end connection
					client.Close();
				}

			}
			catch (Exception ex)
			{
			}
		}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			//byte[] msg = System.Text.Encoding.ASCII.GetBytes(txtMessageToSend.Text);

			// Send the message
			//stream.Write(msg, 0, msg.Length);

			try
			{

				//---create a TCPClient object at the IP and port no.---	
				if (client == null)
					CreateConnection();
				
				NetworkStream nwStream = client.GetStream();
				byte[] bytesToSend = new byte[8];
				bytesToSend[1] = 1;
				bytesToSend[6] = 2;

				//---send the text---
				//_senderWorker.ReportProgress(0, string.Format("Sending bytes[1]= {0} and bytes[6] = {1}: ", bytesToSend[1].ToString(), bytesToSend[6].ToString()));
				nwStream.Write(bytesToSend, 0, bytesToSend.Length);
			}
			catch (Exception ex)
			{

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
			client = new TcpClient("10.120.8.105", 2000);
		}
	}
}
