using Cliente.Listeners;
using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
    class Program
    {
        static TcpListener cliente = null;


        static void Main(string[] args)
        {

            Console.WriteLine("Connectando com servidor");
            TCPListener server = new TCPListener("127.0.0.1", 7171, 1024, Server_OnReceiveHandler);
            server.Connect();

            string texto = "";
            while (texto != "fim")
            {
                texto = Console.ReadLine();
             //   Console.WriteLine(texto);
             
            }

        }

        private static void Server_OnReceiveHandler(ConReader reader)
        {
            Console.WriteLine("Aguardando");

//            Console.WriteLine(reader.GetTag() + " "+ reader.GetBuffer().Length);
            Console.WriteLine(new
            {
                id = reader.ReadInt(),
                Name = reader.ReadString(),
                Vector3 = new[] { reader.ReadLong(), reader.ReadLong(), reader.ReadLong() },
                isPlayer = reader.ReadBool()
            });

        }
    }
}
