using PLCWebControl.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PLCWebControl.Services
{
    public class TcpService : ITcpService
    {
        private TcpClient _client;
        private StateDataModel _lastData = new StateDataModel();
        private NetworkStream stream;

        public TcpService() {
            new Thread(Connect).Start();
        }

        public void Connect()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 3000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];

                // Enter the listening loop.
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();

                    // Get a stream object for reading and writing
                    stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        _lastData.Buttons = bytes[0];
                        _lastData.Sensors = bytes[1];
                        _lastData.WaterLevel = bytes[2];
                        _lastData.Inflow = bytes[3];
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

        public StateDataModel GetLastData()
        {
            return _lastData;
        }

        public void SendData(CommandDataModel command)
        {
            if (_client == null)
            {
                _client = new TcpClient("127.0.0.1", 2000);
            }

            NetworkStream nwStream = _client.GetStream();
            byte[] bytesToSend = new byte[2] { command.Buttons, command.DesiredInflow };

            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }
    }
}