using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces
{
    interface IConReader
    {
        byte[] GetBuffer();
        string ReadString();
        int ReadInt(); 
        float ReadFloat();
        long ReadLong();
        byte[] ReadBytes();
    }
}
