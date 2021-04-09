using Shared.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;

namespace Shared
{
    public class ConWriter : IConWriter, IDisposable
    {
        public byte[] _buffer;
        public int _position = 0;
        public Socket _socket;
        public Encoding _encoding = Encoding.UTF8;
        public int _tag = 0;

        public ConWriter(Socket socket, int tag)
        {
            _buffer = new byte[0];
            _socket = socket;
            _tag = tag;
            Send(tag);
        }
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public void Send(string value)
        {
            byte[] buffer = _encoding.GetBytes(value);
            Send(buffer);
        }

        public void Send(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            ResizeBuffer(buffer.Length);
            Buffer.BlockCopy(buffer, 0, _buffer, _position, buffer.Length);
            _position += buffer.Length;
        }

        public void Send(float value)
        {
        }

        public void Send(long value)
        {
        }


        public void Send(byte[] value)
        {
            Send(value.Length);
            ResizeBuffer(value.Length);
            Buffer.BlockCopy(value, 0, _buffer, _position, value.Length);
        }

        public void Go()
        {
            try
            {
                /*if (_socket.Poll(-1, SelectMode.SelectWrite))
                {*/
                    _socket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, null,null);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void onSend(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);

        }

        public void ResizeBuffer(int size)
        {
            Array.Resize(ref _buffer, _buffer.Length + size);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

       
    }
}
