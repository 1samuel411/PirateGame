using System;
using System.Linq;
using System.Net.Sockets;

namespace SNetwork.Server
{
    public class ServerManager
    {
        private static ServerManager instance;
        private ServerResponseHandler _serverResponseHandler;
        public Server server;

        public static void Main(string[] args)
        {
            var serverManager = new ServerManager();
            serverManager.Start();
        }

        private void Start()
        {
            ResponseManager.instance.Clear();

            server = new Server();
            _serverResponseHandler = new ServerResponseHandler(server);
            _serverResponseHandler.Initialize();
            InitializeCommands();

            instance = this;

            Create("127.0.0.1", 1525, 50000, 2000);
            ListenForCommands();
            return;

            Console.WriteLine("Enter the server's ip");
            var ip = Console.ReadLine();
            Console.WriteLine("Enter the server's port");
            var port = Console.ReadLine();

            var bufferSize = 50000;
            var maxUsers = 1000000;
            var name = "MasterServer";

            Create(ip, int.Parse(port), bufferSize, maxUsers, name);
            ListenForCommands();
        }

        private void ListenForCommands()
        {
            while (true)
            {
                var command = Console.ReadLine();
                if (command.Equals("Help"))
                    Console.WriteLine("The authorities have been alerted!");

                if (command.Equals("Stop"))
                {
                    Console.WriteLine("Stopping");
                    Close();
                }
            }
        }

        public void Create(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000, int maxUsers = 2,
            string name = "")
        {
            server.SetupServer(ip, port, bufferSize, 0.5f, maxUsers, name);
        }

        public void Close()
        {
            server.CloseServer();
        }

        private void InitializeCommands()
        {
            CommandHandler.AddCommand(new Command("time", TimeCommand));
            CommandHandler.AddCommand(new Command("setname", SetName));
            CommandHandler.AddCommand(new Command("kick", Kick));
            CommandHandler.AddCommand(new Command("leave", Leave));
        }

        private void TimeCommand(string text, Socket fromSocket, int fromId)
        {
            Messaging.instance.SendCommandResponse(DateTime.UtcNow.ToString(), fromId, 0, 0, server.clientSockets);
        }

        private void SetName(string newName, Socket fromSocket, int fromId)
        {
            server.clientSockets[fromSocket].username = newName;
            Console.WriteLine("[SNetworking] Setting MasterNetworkPlayer: " + server.clientSockets[fromSocket].id +
                              ", NetworkPlayername to: " + server.clientSockets[fromSocket].username);
            Messaging.instance.SendCommandResponse("[Success]", server.clientSockets[fromSocket].id, 0, 0,
                server.clientSockets);
        }

        private void Kick(string name, Socket fromSocket, int fromId)
        {
            var NetworkPlayerSelected = from x in server.clientSockets where x.Value.username == name select x;

            if (NetworkPlayerSelected.Any())
                Console.WriteLine("[SNetworking] Could not find MasterNetworkPlayer with the name: " + name);
            else
                for (var i = 0; i < NetworkPlayerSelected.Count(); i++)
                {
                    Console.WriteLine("[SNetworking] Kicking MasterNetworkPlayer: " +
                                      NetworkPlayerSelected.ElementAt(i).Value.id + ", with the NetworkPlayername: " +
                                      name);
                    Messaging.instance.SendCommandResponse("[Kicked]", 2, 0, 0, server.clientSockets);

                    server.RemoveSocket(NetworkPlayerSelected.ElementAt(i).Key);
                    NetworkPlayerSelected.ElementAt(i).Key.Shutdown(SocketShutdown.Both);
                    NetworkPlayerSelected.ElementAt(i).Key.Close();
                }
        }

        private void Leave(string text, Socket fromSocket, int fromId)
        {
            Console.WriteLine(
                "[SNetworking] MasterNetworkPlayer: " + server.clientSockets[fromSocket].id + " has left.");
            server.RemoveSocket(fromSocket);
            fromSocket.Disconnect(false);
            fromSocket.Shutdown(SocketShutdown.Both);
            fromSocket.Close();
        }
    }
}