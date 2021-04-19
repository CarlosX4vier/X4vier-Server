using Shared;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using static Servidor.Listeners.TCPListener;

namespace Servidor.Clients
{
    public class TcpClient
    {
        private byte[] _buffer;
        public NetworkStream _stream;
        public Socket _socket;

        private OnReceiveHandler OnReceivedHandler;

        public TcpClient(Socket socket, int maxBufferSize, OnReceiveHandler onReceivedHandler)
        {
            _buffer = new byte[maxBufferSize];
            _socket = socket;
            _stream = new NetworkStream(_socket);

            OnReceivedHandler = onReceivedHandler;
            _stream.BeginRead(_buffer, 0, maxBufferSize, new AsyncCallback(onReceive), null);
        }

        public void onReceive(IAsyncResult ar)
        {
            try
            {
                int size = _stream.EndRead(ar);
                int position = 0;

                if (_socket.Connected)
                {
                    if (size > 0)
                    {
                        while (size != 0)
                        {
                            Options options = new Options();
                            options.buffer = new byte[_server.ReceiveBufferSize];

                            Buffer.BlockCopy(_buffer, position, options.buffer, 0, size);

                            ConReader reader = new ConReader(_socket, options.buffer);
                            position = reader.GetSize();

                            OnReceivedHandler(reader);
                            size = size - position;
                        }
                        _stream.BeginRead(_buffer, 0, _server.ReceiveBufferSize, new AsyncCallback(onReceive), null);

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
                Console.WriteLine("Fechando conexao");
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

                _stream.Write(bufferAux, 0, bufferAux.Length);
                _stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Fechando conexao " + e.Message);
                _socket.Close();
            }
        }
    }
}
