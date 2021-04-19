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


        public ConWriter()
        {
            _buffer = new byte[0];

        }

        public ConWriter(int tag)
        {
            _buffer = new byte[0];
            _tag = tag;
            Send(tag);
        }

   
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public void SetBuffer(byte[] buffer)
        {
            ResizeBuffer(buffer.Length);
            _buffer = buffer;
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
            byte[] buffer = BitConverter.GetBytes(value);
            ResizeBuffer(buffer.Length);
            Buffer.BlockCopy(buffer, 0, _buffer, _position, buffer.Length);
            _position += buffer.Length;
        }

        public void Send(long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            ResizeBuffer(buffer.Length);
            Buffer.BlockCopy(buffer, 0, _buffer, _position, buffer.Length);
            _position += buffer.Length;
        }


        public void Send(byte[] value)
        {
            Send(value.Length);
            ResizeBuffer(value.Length);
            Buffer.BlockCopy(value, 0, _buffer, _position, value.Length);
            _position += value.Length;
        }

        public void Send(bool value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            ResizeBuffer(buffer.Length);
            Buffer.BlockCopy(buffer, 0, _buffer, _position, buffer.Length);
            _position += buffer.Length;
        }

       /* public int Go()
        {
            try
            {

                byte[] bufferAux = BitConverter.GetBytes(_buffer.Length);
                Array.Resize(ref bufferAux, _buffer.Length + 4);
                Array.Copy(_buffer,0,bufferAux,4,_buffer.Length);

                var t = _socket.Send(_buffer);
                return t;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }*/

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