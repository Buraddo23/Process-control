using System;
using System.Net;
using System.Net.Sockets;

namespace TCP_PLC
{
    public class Listener
	{	
		public void Listen()
		{
			TcpListener server = null;
			try
			{				
				Int32 port = 2000;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");
								
				server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];

                // Enter the listening loop.
                while (true)
				{                
                    Console.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();               
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    
					int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
                        Console.WriteLine("I0: {0}\tI1: {1}\tI2: {2}\tI3: {3}\nO0: {4}\tO1: {5}\n", bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]);
                    }

                    // Shutdown and end connection
                    client.Close();
				}
			}
			catch (SocketException e)
			{              
                Console.WriteLine(string.Format("SocketException: {0}", e.ToString()));
			}
			finally
			{
                // Stop listening for new clients.
                server.Stop();
			}

            Console.WriteLine("Hit enter to continue...");
            Console.Read();
		}
	}	
}
