using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TcpClient = Servidor.Clients.TCPClient;

namespace Servidor.Listeners
{

    public class TCPListener
    {
        public static Socket _server = null;
        public IPAddress _ipAddress = null;
        public int _port = 0;
        public byte[] _buffer;
        public ManualResetEvent _mre = new ManualResetEvent(false);

        public delegate void OnReceivedHandler(IClient client, ConReader reader);
        public delegate void OnAcceptHandler(IClient client);
        public delegate void OnDisconnectHandler(IClient client);

        public OnReceivedHandler _onReceivedHandler;
        public OnAcceptHandler _onAcceptHandler;
        public OnDisconnectHandler _onDisconnectHandler;

        public TCPListener(string ip, int port, int maxBufferMessage, OnAcceptHandler onAcceptHandler, OnReceivedHandler onReceivedHandler, OnDisconnectHandler onDisconnectHandler)
        {
            _onReceivedHandler = onReceivedHandler;
            _onAcceptHandler = onAcceptHandler;
            _onDisconnectHandler = onDisconnectHandler;

            _ipAddress = IPAddress.Any;
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _port = port;
            _server.ReceiveBufferSize = maxBufferMessage;
            _server.SendBufferSize = maxBufferMessage;

            _buffer = new byte[maxBufferMessage];
        }

        public Socket GetSocket()
        {
            return _server;
        }
        public void StartServer()
        {
            _server.Bind(new IPEndPoint(_ipAddress, _port));
            _server.Listen(50);
            _server.BeginAccept(new AsyncCallback(Accept), null);
        }

        private void Accept(IAsyncResult ar)
        {
            Socket socket = _server.EndAccept(ar);

            TcpClient client = new TcpClient(socket, _server.ReceiveBufferSize, _onReceivedHandler, _onDisconnectHandler);

            _onAcceptHandler(client);

            _server.BeginAccept(new AsyncCallback(Accept), null);

        }
    }
}
