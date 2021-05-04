using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using static Servidor.Listeners.UDPListener;

namespace Servidor.Clients
{
    public class UDPClient : IClient
    {
        public Socket _socket;
        EndPoint _endPoint = null;

        Socket IClient._socket { get => _socket; set => _socket = value; }

        public UDPClient(Socket socket, EndPoint endPoint)
        {
            _socket = socket;

            _endPoint = endPoint;
        }

        public void Send(ConWriter writer)
        {
            try
            {
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
