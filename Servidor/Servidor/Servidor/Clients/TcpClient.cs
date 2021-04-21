using Shared;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using static Servidor.Listeners.TCPListener;

namespace Servidor.Clients
{
    public class TcpClient : IClient
    {
        private byte[] _buffer;
        public Socket _socket;
        private static int teste = 0;

        private OnReceivedHandler _onReceivedHandler;

        Socket IClient._socket { get => _socket; set => _socket = value; }

        public TcpClient(Socket socket, int maxBufferSize, OnReceivedHandler onReceivedHandler)
        {
            _buffer = new byte[maxBufferSize];
            _socket = socket;

            _onReceivedHandler = onReceivedHandler;
            _socket.BeginReceive(_buffer, 0, maxBufferSize, SocketFlags.None, new AsyncCallback(OnReceived), null);
        }

        public void OnReceived(IAsyncResult ar)
        {
            try
            {
                int size = _socket.EndReceive(ar);

                if (_socket.Connected)
                {
                    if (size > 0)
                    {
                        int position = 0;

                        while (size > position)
                        {
                            byte[] buffer = new byte[_server.ReceiveBufferSize];
                            Buffer.BlockCopy(_buffer, position, buffer, 0, size);

                            ConReader reader = new ConReader(buffer);
                            reader.endPoint = _socket.RemoteEndPoint;

                            //{
                            //    Options options = new Options();
                            //    options.buffer = new byte[_server.ReceiveBufferSize];
                            //    Buffer.BlockCopy(_buffer, position, options.buffer, 0, size);
                            //    ConReader reader = new ConReader(_socket, options.buffer);
                            Console.WriteLine(teste + "- POSITION =" + position + " size = " + size);

                            _onReceivedHandler(this,reader);
                            position += reader.GetSize();

                        }
                        teste++;

                        _socket.BeginReceive(_buffer, 0, _socket.ReceiveBufferSize,SocketFlags.None, new AsyncCallback(OnReceived), null);

                    }   
                    else
                    {
                        Console.WriteLine("Fechando conexao");
                        _socket.Close();
                    }
                }
                else
                {
                    Console.WriteLine("Fechando conexao");
                    _socket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao : "+e.Message);
                _socket.Close();
            }

        }

        public void Send(ConWriter writer)
        {
            try
            {
                Console.WriteLine(writer._buffer.Length + 4);
                byte[] bufferAux = BitConverter.GetBytes(writer._buffer.Length+ 4);
                Array.Resize(ref bufferAux, writer._buffer.Length + 4);
                Array.Copy(writer._buffer, 0, bufferAux, 4, writer._buffer.Length);

                _socket.Send(bufferAux);

            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao " + e.Message);
                _socket.Close();
            }
        }
    }
}
