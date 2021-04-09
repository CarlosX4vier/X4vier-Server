using Servidor.Listeners;
using Shared;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Servidor
{
    class Program
    {
 
        public static TCPListener server = new TCPListener("127.0.0.1", 7171);

        static void Main(string[] args)
        {

             Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Iniciando servidor");
                server.StartServer();
                Console.WriteLine("Servidor iniciado");

                server.OnReceiveHandler += Server_OnReceiveHandler;
                while (true)
                {
                 server.WaitConnection();

                }
            });

            while (Console.ReadLine() != "fim") { } ;

        }



        private static void SendToAll(byte[] buffer)
        {

            foreach (var client in server.clients)
            {
               
                using (ConWriter conWriter = new ConWriter(client))
                {
                    conWriter.Send(buffer);
                    conWriter.Go();
                }
            }
        }

        private static void Server_OnReceiveHandler(object sender, EventArgs e)
        {
            ConReader t = (ConReader)e;
         
            string texto = t.ReadString();
            Console.Out.WriteLine("Mensagem: "+texto);
            if (texto != null)
                SendToAll(t.GetBuffer());

        }
    }
}
