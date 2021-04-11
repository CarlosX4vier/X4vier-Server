using Servidor.Listeners;
using Shared;
using System;
using System.Collections.Generic;
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



        private static void SendToAll(byte[] buffer, int tag, Socket[] exceptions = null)
        {
            List<Socket> clients = server.clients;
            if(exceptions != null)
            {
                clients = clients.Except(exceptions).ToList();
            }

            foreach (var client in clients)
            {
               
                using (ConWriter conWriter = new ConWriter(client,tag))
                {
                    conWriter.Send(buffer);
                    conWriter.Go();
                }
            }
        }

        private static void Server_OnReceiveHandler(object sender, EventArgs e)
        {         

            using (ConReader t = (ConReader)e)
            {
                switch (t.GetTag())
                {
                    case 1:
                        Console.WriteLine("ESCREVENDO");
                        SendToAll(t.GetBuffer(),1, new Socket[]{t.GetSocket()});
                    break;

                    default:
                        break;
                }

            }

           

        }
    }
}
