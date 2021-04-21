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
            _server.BeginConnect(_ipAddress, _port, new AsyncCallback(OnConnect), null);
        }

        public void OnConnect(IAsyncResult a)
        {
            _server.EndConnect(a);

            Console.WriteLine("Conectado");

            _server.BeginReceive(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, new AsyncCallback(OnReceived), null);
        }

        private void OnReceived(IAsyncResult ar)
        {
            int size = _server.EndReceive(ar);

            if (size > 0)
            {
                int position = 0;
                while (size > position)
                {
                    byte[] buffer = new byte[_server.ReceiveBufferSize];
                    Buffer.BlockCopy(_buffer, position, buffer, 0, size);

                    ConReader reader = new ConReader(buffer);
                    reader.endPoint = _server.RemoteEndPoint;

                    Console.WriteLine(teste + "- POSITION =" + position + " size = " + size);

                    _onReceivedHandler(reader);
                    position += reader.GetSize();
                }
                teste++;

                _server.BeginReceive(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, new AsyncCallback(OnReceived), null);
            }

        }

        public void Send(ConWriter writer)
        {
            try
            {
                Console.WriteLine(writer._buffer.Length + 4);
                byte[] bufferAux = BitConverter.GetBytes(writer._buffer.Length + 4);
                Array.Resize(ref bufferAux, writer._buffer.Length + 4);
                Array.Copy(writer._buffer, 0, bufferAux, 4, writer._buffer.Length);

                _server.Send(bufferAux);

            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao " + e.Message);
                _server.Close();
            }

        }
    }
}
