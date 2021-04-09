using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Cliente.Listeners
{
    public class TCPListener
    {
        private Socket _server = null;
        private IPAddress _ipAddress = null;
        private int _port = 0;
        private ManualResetEvent _mre = new ManualResetEvent(false);

        public event EventHandler OnReceiveHandler = delegate { /*Console.WriteLine("Teste mensagem"); */};
        public TCPListener(string ip, int port)
        {
            _ipAddress = IPAddress.Parse(ip);
            _server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _port = port;
        }

        public Socket GetSocket()
        {
            return _server;
        }
        public void Connect()
        {
            _mre.Reset();
            _server.BeginConnect(_ipAddress, _port, new AsyncCallback(onConnect), _server);
            _mre.WaitOne();
        }

        public void onConnect(IAsyncResult a)
        {
            Socket socket = (Socket)a.AsyncState;
            _mre.Set();
            socket.EndConnect(a);
                    
            Console.WriteLine("Conectado");

            Options options = new Options
            {
                buffer = new byte[socket.ReceiveBufferSize],
                socket = socket
            };
            socket.BeginReceive(options.buffer, 0, options.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), options);
        }

        private void OnReceive(IAsyncResult ar)
        {
            Options options = (Options)ar.AsyncState;
            Socket client = options.socket;
            int read = client.EndReceive(ar);

            OnReceiveHandler.Invoke(this, new ConReader(client, options.buffer));

            options.buffer = new byte[client.ReceiveBufferSize];
            client.BeginReceive(options.buffer, 0, options.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), options);

        }
    }
}
