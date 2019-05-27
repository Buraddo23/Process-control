using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;

namespace TCP_PLC
{
    public class Sender
	{		
		public void Send()
		{
			try
			{
				TcpClient client = new TcpClient("127.0.0.1", 3000);

				while (true)
				{
					NetworkStream nwStream = client.GetStream();

					byte[] bytesToSend = new byte[6];
                    bytesToSend[0] = Program.GetInputs()[0];
                    bytesToSend[1] = Program.GetInputs()[1];
                    bytesToSend[2] = Program.GetInputs()[2];
                    bytesToSend[3] = Program.GetInputs()[3];
                    bytesToSend[4] = Program.GetOutputs()[0];
                    bytesToSend[5] = Program.GetOutputs()[1];

					nwStream.Write(bytesToSend, 0, bytesToSend.Length);

					//Thread.Sleep(2000);
				}
			}
			catch (Exception ex)
			{
                Console.WriteLine(string.Format("Exception: {0}", ex.ToString()));
            }
        }
	}
}
