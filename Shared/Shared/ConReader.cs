using Shared.Interfaces;
using System;
using System.Text;

namespace Shared
{
    public class ConReader : EventArgs, IConReader, IDisposable
    {
        private byte[] _buffer;
        private int _position = 0;
        private Encoding _encoding = Encoding.UTF8;
        private int _tag = 0;
        private int _size = 0;


        public ConReader(byte[] buffer)
        {
            _buffer = buffer;
            _size = ReadInt();
            _tag = ReadInt();
        }

        public int GetSize()
        { return _size; }

      
        public int GetTag()
        {
            return _tag;
        }

        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public bool ReadBool()
        {
            bool value = BitConverter.ToBoolean(_buffer, _position);
            _position += 1;
            return value;
        }
        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(_buffer, _position);
            _position += 4;
            return value;
        }

        public int ReadInt()
        {
            int value = BitConverter.ToInt32(_buffer, _position);
            _position += 4;
            return value;
        }

        public long ReadLong()
        {
            long value = BitConverter.ToInt64(_buffer, _position);
            _position += 8;
            return value;
        }

        public string ReadString()
        {
            byte[] byteString = ReadBytes();
            string value = _encoding.GetString(byteString);
            return value;
        }

        public byte[] ReadBytes()
        {
            int size = ReadInt();
            byte[] byteString = new byte[size];
            Array.Copy(_buffer, _position, byteString, 0, size);
            _position += size;
            byte[] value = byteString;
            return value;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }
}
