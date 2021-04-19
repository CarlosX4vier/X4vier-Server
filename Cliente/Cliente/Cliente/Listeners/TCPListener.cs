using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cliente.Listeners
{
    public class TCPListener
    {
        private Socket _server = null;
        private IPAddress _ipAddress = null;
        private int _port = 0;
        private byte[] _buffer;
        private NetworkStream _stream;
        public static int teste = 0;

        public delegate void OnReceivedHandler(ConReader reader);

        public OnReceivedHandler _onReceivedHandler;

        public TCPListener(string ip, int port, int maxBufferMessage, OnReceivedHandler onReceiveHandler)
        {
            _ipAddress = IPAddress.Parse(ip);
            _server = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _port = port;
            _server.ReceiveBufferSize = maxBufferMessage;
            _server.SendBufferSize = maxBufferMessage;

            _onReceivedHandler = onReceiveHandler;

            _buffer = new byte[maxBufferMessage];

        }

        public Socket GetSocket()
        {
            return _server;
        }
        public void Connect()
        {
            _server.BeginConnect(_ipAddress, _port, new AsyncCallback(onConnect), null);
        }

        public void onConnect(IAsyncResult a)
        {
            _server.EndConnect(a);
            _stream = new NetworkStream(_server);

            Console.WriteLine("Conectado");

            _stream.BeginRead(_buffer, 0, _server.ReceiveBufferSize, new AsyncCallback(onReceive), null);
        }

        private void onReceive(IAsyncResult ar)
        {
            int size = _stream.EndRead(ar);
           

            //byte[] buffer = new byte[read];

            //Buffer.BlockCopy(_buffer, 0, buffer, 0, read);
            //ConReader reader = new ConReader(_server, buffer);
            //_onReceivedHandler(reader);

            //_stream.BeginRead(_buffer, 0, _server.ReceiveBufferSize, new AsyncCallback(OnReceive), null);

            if (size > 0)
            {
                int position = 0;
                while (size > position)
                {
                    byte[] buffer = new byte[_server.ReceiveBufferSize];
                    Buffer.BlockCopy(_buffer, position, buffer, 0, size);

                    ConReader reader = new ConReader(_server, buffer);

                    //Console.WriteLine(teste + "- POSITION =" + position + " size = " + size);

                    _onReceivedHandler(reader);
                    position += reader.GetSize();
                }
                teste++;

                _stream.BeginRead(_buffer, 0, _server.ReceiveBufferSize, new AsyncCallback(onReceive), null);
            }

        }

        public void Send(ConWriter writer)
        {
            _stream.BeginWrite(writer.GetBuffer(), 0, writer.GetBuffer().Length, null, null);

        }
    }
}
