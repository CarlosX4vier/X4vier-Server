using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cliente.Listeners
{
    public class UDPListener
    {
        private Socket _server = null;
        private IPAddress _ipAddress = null;
        private int _port = 0;
        private byte[] _buffer;
        public static int teste = 0;

        public EndPoint _endPoint;
        public delegate void OnReceivedHandler(ConReader reader);

        public OnReceivedHandler _onReceivedHandler;

        public UDPListener(string ip, int port, int maxBufferMessage, OnReceivedHandler onReceiveHandler)
        {
            _ipAddress = IPAddress.Parse(ip);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _port = port;
            _server.ReceiveBufferSize = maxBufferMessage;
            _server.SendBufferSize = maxBufferMessage;

            _onReceivedHandler = onReceiveHandler;

            _buffer = new byte[maxBufferMessage];
            _server.Bind(new IPEndPoint(IPAddress.Any,0));
            _endPoint = new IPEndPoint(_ipAddress, _port);

            _server.BeginReceiveFrom(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, ref _endPoint, new AsyncCallback(OnReceived), null);


        }

        public Socket GetSocket()
        {
            return _server;
        }


        private void OnReceived(IAsyncResult ar)
        {
            EndPoint endPoint = new IPEndPoint(_ipAddress, _port);
            int size = _server.EndReceiveFrom(ar, ref endPoint);

            if (size > 0)
            {
                int position = 0;
              
                    byte[] buffer = new byte[_server.ReceiveBufferSize];
                    Buffer.BlockCopy(_buffer, position, buffer, 0, size);

                    ConReader reader = new ConReader(buffer);

                    Console.WriteLine(teste + "- POSITION =" + position + " size = " + size);

                    _onReceivedHandler(reader);
                    position += reader.GetSize();
                
                teste++;

            }
            _server.BeginReceiveFrom(_buffer, 0, _server.ReceiveBufferSize, SocketFlags.None, ref _endPoint, new AsyncCallback(OnReceived), null);

        }

        public void Send(ConWriter writer)
        {
            try
            {
                Console.WriteLine(writer._buffer.Length + 4);
                byte[] bufferAux = BitConverter.GetBytes(writer._buffer.Length + 4);
                Array.Resize(ref bufferAux, writer._buffer.Length + 4);
                Array.Copy(writer._buffer, 0, bufferAux, 4, writer._buffer.Length);

                EndPoint p = new IPEndPoint(_ipAddress, _port);
                _server.SendTo(bufferAux, SocketFlags.None, p);

            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
