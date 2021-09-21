using Servidor.Clients;
using Servidor.Listeners;
using Servidor.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servidor
{
    class Program
    {
        public static int idPlayer = 0;
        public static Dictionary<IClient, Player> clientsTCP = new Dictionary<IClient, Player>();
        public static Dictionary<IClient, Player> clientsUDP = new Dictionary<IClient, Player>();
        public static UDPListener serverUDP;
        public static TCPListener serverTCP;

        public static List<Match> Matches = new List<Match>();

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
                serverUDP = new UDPListener("192.168.0.100", 7172, 1024, Server_OnAcceptHandler, Server_OnReceiveHandler);
                serverUDP.StartServer();

                serverTCP = new TCPListener("192.168.0.100", 7171, 1024, Server_OnAcceptHandler, Server_OnReceiveHandler, Server_OnDisconnectHandler);
                serverTCP.StartServer();

                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> Servidor iniciado");


            });

            while (Console.ReadLine() != "fim") { };

        }

        private static void Server_OnDisconnectHandler(IClient client)
        {
            clientsTCP.Remove(client);
        }

        private static void Server_OnAcceptHandler(IClient client)
        {
            Player player = new Player();
            player.Id = idPlayer;
            player.Name = "Carlos " + idPlayer;

            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> " + client._socket.LocalEndPoint + " conectou");

            clientsTCP.Add(client, player);

            //Mando para o player todos os usuarios
            foreach (var p in clientsTCP)
            {

                using (ConWriter writer = new ConWriter(1))
                {
                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ">  1 - Enviando " + p.Value.Id + " para " + player.Id);
                    writer.Send(p.Value.Id);
                    writer.Send(p.Value.Name);
                    writer.Send(0F);
                    writer.Send(0F);
                    writer.Send(0F);

                    writer.Send(p.Key == client);

                    client.Send(writer);
                }
            }


            //Manda todos os players  o novo player
            foreach (var p in clientsTCP)
            {
                if (p.Key != client)
                    using (ConWriter writer = new ConWriter(1))
                    {
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "> 2 - Enviando " + player.Id + " para " + p.Value.Id);

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


            foreach (var client in clientsTCP)
            {
                if (!exceptions.Contains(client.Key))
                {
                    using (ConWriter conWriter = new ConWriter())
                    {
                        conWriter.SetBuffer(buffer);
                        client.Key.Send(conWriter);
                    }
                }
            }
        }

        private static void Server_OnReceiveHandler(IClient client, ConReader reader)
        {

            using (ConReader t = reader)
            {
                int tag = t.GetTag();

                if (client is UDPClient && !clientsUDP.ContainsKey(client))
                {
                    Console.WriteLine("Adicionando no UDP");
                    clientsUDP.Add(client, new Player());
                }

                if (client is TCPClient)
                {
                    switch (tag)
                    {
                        case 1:

                            int id = t.ReadInt();
                            string nome = t.ReadString();
                            float x = t.ReadFloat();
                            float y = t.ReadFloat();
                            float z = t.ReadFloat();
                            bool isLocal = t.ReadBool();

                            foreach (var p in clientsTCP)
                            {
                                using (ConWriter conWriter = new ConWriter(1))
                                {
                                    conWriter.Send(id);
                                    conWriter.Send(nome);
                                    conWriter.Send(x);
                                    conWriter.Send(y);
                                    conWriter.Send(z);
                                    conWriter.Send(isLocal);
                                    p.Key.Send(conWriter);
                                }
                            }

                            break;
                        case 2:
                            using (ConWriter writer = new ConWriter(2))
                            {
                                int playerId = t.ReadInt();
                                KeyValuePair<IClient, Player> player = clientsTCP.First(x1 => { Console.WriteLine(x1.Value.Id + " " + playerId); return x1.Value.Id == playerId; });
                                player.Value.Position.X = t.ReadFloat();
                                player.Value.Position.Y = t.ReadFloat();
                                player.Value.Position.Z = t.ReadFloat();

                                writer.Send(player.Value.Id);
                                writer.Send(player.Value.Position.X);
                                writer.Send(player.Value.Position.Y);
                                writer.Send(player.Value.Position.Z);

                                Console.WriteLine("Enviando para todos mensagem de " + player.Value.Id);
                                SendToAll(writer.GetBuffer(), new List<IClient>() { player.Key });

                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    int playerId = reader.ReadInt();
                    switch (tag)
                    {
                        case 1:
                            Console.WriteLine(reader.ReadString());
                            break;
                    }
                }

            }
        }
    }
}
