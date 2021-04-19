using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpClient = Servidor.Clients.TcpClient;

namespace Servidor.Listeners
{

   public class TCPListener
    {
        public static Socket _server = null;
        public IPAddress _ipAddress = null;
        public int _port = 0;
        public byte[] _buffer;
        public ManualResetEvent _mre = new ManualResetEvent(false);

        public delegate void OnReceiveHandler(ConReader reader);
        public delegate void OnAcceptHandler (TcpClient socket);
        public delegate void OnDisconnectHandler();

        public OnReceiveHandler _onReceiveHandler;
        public OnAcceptHandler _onAcceptHandler;

        public TCPListener(string ip, int port, int maxBufferMessage, OnAcceptHandler onAcceptHandler, OnReceiveHandler onReceiveHandler)
        {
            _onReceiveHandler = onReceiveHandler;
            _onAcceptHandler = onAcceptHandler;

            _ipAddress = IPAddress.Any;
            _server = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
        }

        public void WaitConnection()
        {
            _mre.Reset();
            _server.BeginAccept(new AsyncCallback(Accept), null);
            _mre.WaitOne();
        }

        private void Accept(IAsyncResult ar)
        {
            Socket socket = _server.EndAccept(ar);

            TcpClient client = new TcpClient(socket,1024, _onReceiveHandler);

            _onAcceptHandler(client);

            _server.BeginAccept(new AsyncCallback(Accept), null);

        }   

        private void Disconnect(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " desconectado");
            socket.Close();
        }
    }
}
