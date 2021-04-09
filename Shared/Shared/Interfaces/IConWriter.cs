using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces
{
    interface IConWriter
    {
        byte[] GetBuffer();
        void Send(string value);
        void Send(int value);
        void Send(float value);
        void Send(long value);
        void Send(byte[] value);
    }
}
