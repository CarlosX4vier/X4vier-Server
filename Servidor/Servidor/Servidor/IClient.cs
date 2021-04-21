using Shared;
using System;
using System.Net.Sockets;

namespace Servidor
{
    public interface IClient
    {
        Socket _socket { get; set; }

        void Send(ConWriter writer);
    }
}
