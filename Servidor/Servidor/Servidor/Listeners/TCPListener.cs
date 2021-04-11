using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servidor.Listeners
{

    class TCPListener
    {
        public static Socket _server = null;
        public IPAddress _ipAddress = null;
        public int _port = 0;
        public byte[] _buffer;
        public ManualResetEvent _mre = new ManualResetEvent(false);

        public event EventHandler OnReceiveHandler = delegate { };

        public List<Socket> clients = new List<Socket>();

        public TCPListener(string ip, int port)
        {
            _ipAddress = IPAddress.Any;
            _server = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _port = port;

        }

        public Socket GetSocket()
        {
            return _server;
        }
        public void StartServer()
        {
            _server.Bind(new IPEndPoint(_ipAddress, _port));
            _server.Listen(50);
        }

        public void WaitConnection()
        {
            Console.WriteLine("Aguardando conexao com cliente");
            _mre.Reset();
            _server.BeginAccept(new AsyncCallback(Accept), _server);
            _mre.WaitOne();
        }

        private void Accept(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            _mre.Set();
            Socket client = socket.EndAccept(ar);
          //  client.BeginDisconnect(false, new AsyncCallback(Disconnect), client);
            clients.Add(client);

            Options options = new Options()
            {
                buffer = new byte[client.ReceiveBufferSize],
                socket = client
            };

            client.BeginReceive(options.buffer, 0, options.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), options);

        }

        private void OnReceive(IAsyncResult ar)
        {
            Options options = (Options)ar.AsyncState;
            Socket client = options.socket;

            try
            {
                int read = client.EndReceive(ar);
                Console.WriteLine(client.RemoteEndPoint.ToString() + " " + client.Connected);

                if (client.Connected)
                {
                    if (read > 0)
                    {
                        OnReceiveHandler.Invoke(this, new ConReader(client, options.buffer));

                        byte[] teste = new byte[client.ReceiveBufferSize];
                        options.buffer = teste;

                        client.BeginReceive(teste, 0, teste.Length, SocketFlags.None, new AsyncCallback(OnReceive), options);

                    }
                    else
                    {
                        Disconnect(client);
                    }
                }
                else
                {
                    Disconnect(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Disconnect(client);
            }
        }

        private void Disconnect(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            Console.WriteLine(socket.LocalEndPoint.ToString() + " desconectado");

            socket.Close();
            clients.Remove(socket);
        }

        private void Disconnect(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " desconectado");

            socket.Close();
            clients.Remove(socket);
        }
    }
}
