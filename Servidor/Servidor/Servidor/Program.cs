using Servidor.Listeners;
using Servidor.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TcpClient = Servidor.Clients.TcpClient;

namespace Servidor
{
    class Program
    {
        public static int idPlayer = 0;
        public static Dictionary<Player, TcpClient> clients = new Dictionary<Player, TcpClient>();
        public static TCPListener server;

        static void Main(string[] args)
        {
            Console.WriteLine(" ___    ___ ___   ___  ___      ___ ___  _______   ________          ________  _______   ________  ___      ___ _______   ________     ");
            Console.WriteLine(@"|\  \  /  /|\  \ |\  \|\  \    /  /|\  \|\  ___ \ |\   __  \        |\   ____\|\  ___ \ |\   __  \|\  \    /  /|\  ___ \ |\   __  \    ");
            Console.WriteLine(@"\ \  \/  / | \  \\_\  \ \  \  /  / | \  \ \   __/|\ \  \|\  \       \ \  \___|\ \   __/|\ \  \|\  \ \  \  /  / | \   __/|\ \  \|\  \   ");
            Console.WriteLine(@" \ \    / / \ \______  \ \  \/  / / \ \  \ \  \_|/_\ \   _  _\       \ \_____  \ \  \_|/_\ \   _  _\ \  \/  / / \ \  \_|/_\ \   _  _\  ");
            Console.WriteLine(@"  /     \/   \|_____|\  \ \    / /   \ \  \ \  \_|\ \ \  \\  \|       \|____|\  \ \  \_|\ \ \  \\  \\ \    / /   \ \  \_|\ \ \  \\  \| ");
            Console.WriteLine(@" /  /\   \          \ \__\ \__/ /     \ \__\ \_______\ \__\\ _\         ____\_\  \ \_______\ \__\\ _\\ \__/ /     \ \_______\ \__\\ _\ ");
            Console.WriteLine(@"/__/ /\ __\          \|__|\|__|/       \|__|\|_______|\|__|\|__|       |\_________\|_______|\|__|\|__|\|__|/       \|_______|\|__|\|__|");
            Console.WriteLine(@"|__|/ \|__|                                                            \|_________|                                                    ");

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> Iniciando servidor");
                server = new TCPListener("192.168.0.100", 7171, 1024, Server_OnAcceptHandler, Server_OnReceiveHandler);
                server.StartServer();
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> Servidor iniciado");


                //System.Timers.Timer t = new System.Timers.Timer();
                //t.Interval = 2000;
                //t.Elapsed += T_Elapsed;
                //t.Start();

                while (true)
                {
                    server.WaitConnection();

                }
            });

            while (Console.ReadLine() != "fim") { };

        }


        private static void Server_OnAcceptHandler(TcpClient client)
        {

            Player player = new Player();
            player.Id = idPlayer;
            player.Name = "Carlos " + idPlayer;

            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> " + client._socket.LocalEndPoint + " conectou");

            clients.Add(player, client);

            //Mando para o player todos os usuarios
            for (int i = 0; i < 10; i++)
            {
                Player playerTeste = new Player();
                playerTeste.Id = i;
                playerTeste.Name = "Carlos " + i;

                using (ConWriter writer = new ConWriter(1))
                {
                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ">  1 - Enviando " + i + " para " + player.Id);
                    writer.Send(playerTeste.Id);
                    writer.Send(playerTeste.Name);
                    writer.Send(0L);
                    writer.Send(0L);
                    writer.Send(0L);

                    writer.Send(i == idPlayer);
                    
                    client.Send(writer);
                }
            }


            //Manda todos os players  o novo player
           /* foreach (var p in clients)
            {
                if (p.Value != client)
                    using (ConWriter writer = new ConWriter(1))
                    {
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> 2 - Enviando " + player.Id + " para " + p.Key.Id);

                        writer.Send(player.Id);
                        writer.Send(player.Name);
                        writer.Send(player.Position.X);
                        writer.Send(player.Position.Y);
                        writer.Send(player.Position.Z);
                        writer.Send(false);

                        client.Send(writer);
                        Console.WriteLine("Enviado ");
                        //SendToAll(writer._buffer, new List<Socket> { socket });
                    }
            }
           */
            idPlayer = idPlayer + 1;

        }

        private static void SendToAll(byte[] buffer, List<TcpClient> exceptions = null)
        {
            if (exceptions is null)
            {
                exceptions = new List<TcpClient>();
            }


            foreach (var client in clients)
            {
                if (!exceptions.Contains(client.Value))
                {
                    using (ConWriter conWriter = new ConWriter())
                    {
                        conWriter.SetBuffer(buffer);
                        client.Value.Send(conWriter);
                    }
                }
            }
        }

        private static void Server_OnReceiveHandler(ConReader reader)
        {

            using (ConReader t = reader)
            {
                switch (t.GetTag())
                {
                    case 1:
                        //SendToAll(t.GetBuffer(), new List<Socket> { t.GetSocket() });

                        int id = t.ReadInt();
                        string nome = t.ReadString();
                        long x = t.ReadLong();
                        long y = t.ReadLong();
                        long z = t.ReadLong();
                        bool isLocal = t.ReadBool();

                        foreach (var p in clients)
                        {
                            using (ConWriter conWriter = new ConWriter(1))
                            {
                                conWriter.Send(id);
                                conWriter.Send(nome);
                                conWriter.Send(x);
                                conWriter.Send(y);
                                conWriter.Send(z);
                                conWriter.Send(isLocal);
                                p.Value.Send(conWriter);
                            }
                        }

                        break;
                    case 2:
                        using (ConWriter writer = new ConWriter(2))
                        {
                            /*  Player player = clients.First(x1 => x1.Value == t.()).Key;
                              player.Position.X = t.ReadLong();
                              player.Position.Y = t.ReadLong();
                              player.Position.Z = t.ReadLong();

                              writer.Send(player.Id);
                              writer.Send(player.Position.X);
                              writer.Send(player.Position.Y);
                              writer.Send(player.Position.Z);
                              //  SendToAll(writer.GetBuffer(), new List<Socket>() { t.GetSocket() });
                            */
                        }
                        break;
                    default:
                        break;
                }

            }



        }
    }
}
