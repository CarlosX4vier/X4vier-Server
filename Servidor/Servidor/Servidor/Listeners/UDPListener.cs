using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UdpClient = Servidor.Clients.UdpClient;

namespace Servidor.Listeners
{

    public class UDPListener
    {
        public static Socket _server = null;
        public IPAddress _ipAddress = null;
        public int _port = 0;
        public byte[] _buffer;
        public ManualResetEvent _mre = new ManualResetEvent(false);
        public EndPoint _endPoint;
        public int teste = 0;

        public delegate void OnReceivedHandler(IClient client, ConReader reader);
        public delegate void OnAcceptHandler(IClient socket);
        public delegate void OnDisconnectHandler();

        public OnReceivedHandler _onReceivedHandler;
        public OnAcceptHandler _onAcceptHandler;

        public UDPListener(string ip, int port, int maxBufferMessage, OnAcceptHandler onAcceptHandler, OnReceivedHandler onReceivedHandler)
        {
            _onReceivedHandler = onReceivedHandler;
            _onAcceptHandler = onAcceptHandler;

            _ipAddress = IPAddress.Any;
            _server = new Socket(_ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _port = port;
            _server.ReceiveBufferSize = maxBufferMessage;
            _server.SendBufferSize = maxBufferMessage;
            _endPoint = new IPEndPoint(_ipAddress, port);

            _buffer = new byte[maxBufferMessage];
        }

        public Socket GetSocket()
        {
            return _server;
        }
        public void StartServer()
        {
            _server.Bind(new IPEndPoint(_ipAddress, _port));
            _server.BeginReceiveFrom(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, ref _endPoint, new AsyncCallback(OnReceived), null);

        }
        public void OnReceived(IAsyncResult ar)
        {
            try
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint tempRemoteEP = (EndPoint)sender;

                int size = _server.EndReceiveFrom(ar, ref tempRemoteEP);

                if (size > 0)
                {
                    int position = 0;

                    while (size > position)
                    {
                        byte[] buffer = new byte[_server.ReceiveBufferSize];
                        Buffer.BlockCopy(_buffer, position, buffer, 0, size);
                        ConReader reader = new ConReader(buffer);
                        reader.endPoint = tempRemoteEP;
                        //{
                        //    Options options = new Options();
                        //    options.buffer = new byte[_server.ReceiveBufferSize];
                        //    Buffer.BlockCopy(_buffer, position, options.buffer, 0, size);
                        //    ConReader reader = new ConReader(_socket, options.buffer);
                        Console.WriteLine(teste + "- POSITION =" + position + " size = " + size);

                        UdpClient client = new UdpClient(_server, tempRemoteEP, _server.ReceiveBufferSize);
                        _onReceivedHandler(client, reader);
                        position += reader.GetSize();

                    }
                }
                _server.BeginReceiveFrom(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, ref _endPoint, new AsyncCallback(OnReceived), null);

            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao : " + e.StackTrace);
            }
        }



        private void Disconnect(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " desconectado");
            socket.Close();
        }
    }
}
