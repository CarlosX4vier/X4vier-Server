using Cliente.Listeners;
using Shared;
using System;

namespace Cliente
{
    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine("Connectando com servidor");
            TCPListener servertcp = new TCPListener("127.0.0.1", 7171, 1024, Server_OnReceiveHandler);
            servertcp.Connect();

            UDPListener server = new UDPListener("192.168.0.100", 7172, 1024, Server_OnReceiveHandler);
            for (int i = 0; i < 10; i++)
            {

                using (ConWriter writer = new ConWriter(0))
                {
                    writer.Send("Teste envia UDP" + i);

                    server.Send(writer);
                }
            }

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
