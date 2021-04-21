using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using static Servidor.Listeners.UDPListener;

namespace Servidor.Clients
{
    public class UdpClient : IClient
    {
        private byte[] _buffer;
        public Socket _socket;
        private static int teste = 0;
        EndPoint _endPoint = null;
        private OnReceivedHandler _onReceivedHandler;

        Socket IClient._socket { get => _socket; set => _socket = value; }

        public UdpClient(Socket socket, EndPoint endPoint, int maxBufferSize)
        {
            _buffer = new byte[maxBufferSize];
            _socket = socket;

            _endPoint = endPoint;
        }

        public void Send(ConWriter writer)
        {
            try
            {
                Console.WriteLine(writer._buffer.Length + 4);
                byte[] bufferAux = BitConverter.GetBytes(writer._buffer.Length + 4);
                Array.Resize(ref bufferAux, writer._buffer.Length + 4);
                Array.Copy(writer._buffer, 0, bufferAux, 4, writer._buffer.Length);

                _socket.SendTo(bufferAux, SocketFlags.None, _endPoint);

            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao " + e.StackTrace);
            }
        }
    }
}
