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
        public static Dictionary<Player, IClient> clients = new Dictionary<Player, IClient>();
        public static UDPListener server;

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
                server = new UDPListener("192.168.0.100", 7171, 1024, Server_OnAcceptHandler, Server_OnReceiveHandler);
                server.StartServer();
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> Servidor iniciado");

                while (true)
                {
                   // server.WaitConnection();
                }
            });

            while (Console.ReadLine() != "fim") { };

        }


        private static void Server_OnAcceptHandler(IClient client)
        {

            Player player = new Player();
            player.Id = idPlayer;
            player.Name = "Carlos " + idPlayer;

            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> " + client._socket.LocalEndPoint + " conectou");

            clients.Add(player, client);

            //Mando para o player todos os usuarios
            foreach (var p in clients)
            {

                using (ConWriter writer = new ConWriter(1))
                {
                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ">  1 - Enviando " + p.Key.Id + " para " + player.Id);
                    writer.Send(p.Key.Id);
                    writer.Send(p.Key.Name);
                    writer.Send(0F);
                    writer.Send(0F);
                    writer.Send(0F);

                    writer.Send(p.Value == client);

                    client.Send(writer);
                }
            }


            //Manda todos os players  o novo player
            foreach (var p in clients)
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

                        SendToAll(writer._buffer, new List<IClient>() { client });
                        Console.WriteLine("Enviado ");
                    }
            }

            idPlayer = idPlayer + 1;

        }

        private static void SendToAll(byte[] buffer, List<IClient> exceptions = null)
        {
            if (exceptions is null)
            {
                exceptions = new List<IClient>();
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

        private static void Server_OnReceiveHandler(IClient client, ConReader reader)
        {
            using (ConReader t = reader)
            {

                int tag = t.GetTag();
                Console.WriteLine(tag);
                switch (tag)
                {
                    case 1:
                        //SendToAll(t.GetBuffer(), new List<Socket> { t.GetSocket() });
                        Console.WriteLine(t.ReadString());
                        ConWriter writer2 = new ConWriter(1);
                        writer2.Send("Recebi desgraçado");
                        client.Send(writer2);
                      /*  int id = t.ReadInt();
                        string nome = t.ReadString();
                        float x = t.ReadFloat();
                        float y = t.ReadFloat();
                        float z = t.ReadFloat();
                        bool isLocal = t.ReadBool();

                        foreach (var p in clients)
                        {
                            using (ConWriter conWriter = new ConWriter(1))
                            {
                                conWriter.endPoint = t.endPoint;

                                conWriter.Send(id);
                                conWriter.Send(nome);
                                conWriter.Send(x);
                                conWriter.Send(y);
                                conWriter.Send(z);
                                conWriter.Send(isLocal);
                                p.Value.Send(conWriter);
                            }
                        }
                      */
                        break;
                    case 2:
                        using (ConWriter writer = new ConWriter(2))
                        {

                          /*  KeyValuePair<Player, IClient> player = clients.First(x1 => x1.Val == reader.GetSocket());
                            player.Key.Position.X = t.ReadFloat();
                            player.Key.Position.Y = t.ReadFloat();
                            player.Key.Position.Z = t.ReadFloat();

                            writer.Send(player.Key.Id);
                            writer.Send(player.Key.Position.X);
                            writer.Send(player.Key.Position.Y);
                            writer.Send(player.Key.Position.Z);
                          
                            Console.WriteLine("Enviando para todos mensagem de " + player.Key.Id);
                            SendToAll(writer.GetBuffer(), new List<IClient>() { player.Value });
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
