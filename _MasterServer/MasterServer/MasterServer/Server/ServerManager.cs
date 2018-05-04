using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace SNetwork.Server
{
	public class ServerManager
	{
		public Server server;
        private ServerResponseHandler _serverResponseHandler;

        private static ServerManager instance;

        public static void Main(string[] args)
        {
            ServerManager serverManager = new ServerManager();
            serverManager.Start();
        }

	    void Start()
	    {
	        ResponseManager.instance.Clear();

	        Console.WriteLine("Enter the server's ip");
	        string ip = Console.ReadLine();
	        Console.WriteLine("Enter the server's port");
	        string port = Console.ReadLine();
	        Console.WriteLine("Enter the server's bufferSize (50000 default)");
	        string bufferSize = Console.ReadLine();
	        Console.WriteLine("Enter the server's max users");
	        string maxUsers = Console.ReadLine();
	        Console.WriteLine("Enter the server's name");
	        string name = Console.ReadLine();

	        server = new Server();
	        _serverResponseHandler = new ServerResponseHandler(server);
	        _serverResponseHandler.Initialize();
	        InitializeCommands();

	        instance = this;

	        Create(ip, int.Parse(port), int.Parse(bufferSize), int.Parse(maxUsers), name);
        }

        void ListenForCommands()
	    {
	        while (true)
	        {
	            string command = Console.ReadLine();
	            if (command.Equals("Help"))
	            {
	                Console.WriteLine("The authorities have been alerted!");
	            }

	            if (command.Equals("Stop"))
	            {
	                Console.WriteLine("Stopping");
	                Close();
	            }
            }
	    }
        
		public void Create(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000, int maxUsers = 2, string name = "")
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
            Console.WriteLine("[SNetworking] Setting MasterNetworkPlayer: " + server.clientSockets[fromSocket].id + ", NetworkPlayername to: " + server.clientSockets[fromSocket].username);
            Messaging.instance.SendCommandResponse("[Success]", server.clientSockets[fromSocket].id, 0, 0, server.clientSockets);
        }

	    private void Kick(string name, Socket fromSocket, int fromId)
	    {
            var NetworkPlayerSelected = from x in server.clientSockets where x.Value.username == name select x;

            if (NetworkPlayerSelected.Any())
            {
                Console.WriteLine("[SNetworking] Could not find MasterNetworkPlayer with the name: " + name);
            }
            else
            {
                for (int i = 0; i < NetworkPlayerSelected.Count(); i++)
                {
                    Console.WriteLine("[SNetworking] Kicking MasterNetworkPlayer: " + NetworkPlayerSelected.ElementAt(i).Value.id + ", with the NetworkPlayername: " + name);
                    Messaging.instance.SendCommandResponse("[Kicked]", 2, 0, 0, server.clientSockets);
                    NetworkPlayerSelected.ElementAt(i).Key.Shutdown(SocketShutdown.Both);
                    NetworkPlayerSelected.ElementAt(i).Key.Close();
                }
            }
        }

        private void Leave(string text, Socket fromSocket, int fromId)
        {
            Console.WriteLine("[SNetworking] MasterNetworkPlayer: " + server.clientSockets[fromSocket].id + " has left.");
            server.RemoveSocket(fromSocket);
            fromSocket.Shutdown(SocketShutdown.Both);
            fromSocket.Close();
        }
    }
}