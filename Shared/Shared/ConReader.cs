using Shared.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;

namespace Shared
{
    public class ConReader : EventArgs, IConReader, IDisposable
    {
        private byte[] _buffer;
        private int _position = 0;
        private Socket _socket = null;
        private Encoding _encoding = Encoding.UTF8;
        private int _tag = 0;


        public ConReader(Socket socket)
        {
            _socket = socket;
            _buffer = new byte[_socket.ReceiveBufferSize];
        }

        public ConReader(Socket socket,byte[] buffer)
        {
            _socket = socket;
            _buffer = buffer;
            _tag = ReadInt();
        }

        public int GetTag()
        {
            return _tag;
        }

        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public float ReadFloat()
        {
            throw new NotImplementedException();

        }

        public int ReadInt()
        {
            int value = BitConverter.ToInt32(_buffer, _position);
            _position += 4;
            return value;
        }

        public long ReadLong()
        {
            throw new NotImplementedException();
        }

        public string ReadString()
        {
            int size = ReadInt();
            byte[] byteString = new byte[size];
            Array.Copy(_buffer, _position, byteString, 0, size);
            string value = _encoding.GetString(byteString);
            return value;
        }

        public byte[] ReadBytes()
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

  
    }
}
