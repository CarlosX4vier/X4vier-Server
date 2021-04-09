using Cliente.Listeners;
using Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cliente
{
  /*  class Program
    {
        static Socket cliente = null;

        /*  static void startServer()
          {
              bool connectado = false;
              while (!connectado)
                  try
                  {
                      IPAddress ip = IPAddress.Parse("127.0.0.1");
                      // cliente = new TcpListener(ip, 7171);
                      //cliente.Start();
                      cliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                      cliente.Connect(ip, 7171);
                      connectado = true;
                  }
                  catch (Exception e)
                  {
                      Console.WriteLine("Tentando reconexao");
                      Thread.Sleep(2000);
                  }
          }

        static void Main(string[] args)
        {

            Console.WriteLine("Connectando com servidor");
            TCPListener server = new TCPListener("127.0.0.1", 7171);
            server.Connect();
            server.OnReceiveHandler += Server_OnReceiveHandler;

            using (ConWriter conWriter = new ConWriter(server.GetSocket()))
            {
                int valorAleatorio = new Random(312).Next(0, 1500);
                conWriter.Send(12);
                conWriter.Send(valorAleatorio);
                conWriter.Send("Impossivel ter ido de primeira");
                conWriter.Go();
            }
            
            string texto = "";
            while (texto != "fim")
            {
                texto = Console.ReadLine();
             //   Console.WriteLine(texto);
                using (ConWriter conWriter = new ConWriter(server.GetSocket()))
                {
                    conWriter.Send(texto);
                    conWriter.Go();
                }
            }

            server.GetSocket().Close();
        }

        private static void Server_OnReceiveHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Aguardando");
            ConReader t = (ConReader)e;
       //     int valor = t.ReadInt();
         //   int valorAleatorio = t.ReadInt();
            string texto = t.ReadString();
            Console.WriteLine(texto);
        }
    }*/
}
