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

        static void Main(string[] args)
        {

            Console.WriteLine("Connectando com servidor");
            UDPListener server = new UDPListener("127.0.0.1", 7171, 1024, Server_OnReceiveHandler);

            ConWriter writer = new ConWriter(1);
            writer.Send("Teste");

            server.Send(writer);

            string texto = "";
            while (texto != "fim")
            {
                texto = Console.ReadLine();
                ConWriter writer1 = new ConWriter(1);
                writer1.Send(texto);
                Console.WriteLine("Tamanho do buffer: " + writer1._buffer.Length);
                server.Send(writer1);
            }

        }

        private static void Server_OnReceiveHandler(ConReader reader)
        {
            Console.WriteLine("Aguardando");

            Console.WriteLine(new
            {
                
                Name = reader.ReadString(),
               
            });

        }
    }
}
