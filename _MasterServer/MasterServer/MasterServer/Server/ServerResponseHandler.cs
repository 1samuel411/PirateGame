using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;

namespace SNetwork.Server
{
    public class ServerResponseHandler
    {
        private readonly Server _server;

        public ServerResponseHandler(Server server)
        {
            _server = server;
        }

        public void Initialize()
        {
            ResponseManager.instance.Clear();
            ResponseManager.instance.AddServerResponse(Response21, 21);
            ResponseManager.instance.AddServerResponse(Response20, 20);
            ResponseManager.instance.AddServerResponse(Response12, 12);
            ResponseManager.instance.AddServerResponse(Response3, 3);
            ResponseManager.instance.AddServerResponse(Response2, 2);
            ResponseManager.instance.AddServerResponse(Response50, 50);
            ResponseManager.instance.AddServerResponse(Response72, 72);
            ResponseManager.instance.AddServerResponse(Response73, 73);
            ResponseManager.instance.AddServerResponse(Response74, 74);
        }

        public void Response21(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 21: " + fromId + ": " + ByteParser.ConvertToASCII(responseBytes));
            CommandHandler.RunCommand(ByteParser.ConvertToASCII(responseBytes), fromSocket, fromId);
        }

        public void Response20(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 20: " + ByteParser.ConvertToASCII(responseBytes));
        }

        public void Response12(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 12: " + fromId + ": " + responseBytes.Length);
        }

        public void Response3(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 3: " + fromId + ": " + responseBytes.Length);
            _server.SetUserData(fromId, ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }

        public void Response2(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 2: " + fromId + ": " + responseBytes.Length);
            _server.SetServerData(ByteParser.ConvertDataToKeyValuePair(responseBytes));
        }

        public void Response50(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            Console.WriteLine("Recieved a 50: " + fromId + ": " + responseBytes.Length);
            MasterNetworkPlayer masterNetworkPlayer = ByteParser.ConvertToNetworkPlayer(responseBytes);

            // Check it doesn't exist
            MasterNetworkPlayer check = _server.clientSockets.Values.FirstOrDefault(x => x.playfabId == masterNetworkPlayer.playfabId);
            if (check != null)
            {
                // Exists
                Console.WriteLine("[SNetworking] This user already is loggged in");
                Messaging.instance.SendInfoMessage(fromSocket, "Already Logged In", 0);
                _server.RemoveSocket(fromSocket);
                return;
            }

            ServerManager.instance.server.SetLoggedIn(masterNetworkPlayer.playfabId);
            _server.clientSockets[fromSocket] = masterNetworkPlayer;
            _server.clientSockets[fromSocket].id = fromId;

            

            Console.WriteLine("Users info: " + _server.clientSockets[fromSocket].id + ", " + _server.clientSockets[fromSocket].username + ", " + _server.clientSockets[fromSocket].playfabId);
        }

        public void Response72(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve playfabId to send invite to
            string playfabTarget = ByteParser.ConvertToASCII(responseBytes);
            Console.WriteLine("Recieved invite request to: " + playfabTarget);
            ServerManager.instance.server.InviteToRoom(ServerManager.instance.server.clientSockets[fromSocket].playfabId, playfabTarget);
        }

        public void Response73(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve invite Id to accept
            int inviteTarget = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved accept invite request to: " + inviteTarget);
            ServerManager.instance.server.AcceptInvite(inviteTarget);
        }

        public void Response74(byte[] responseBytes, Socket fromSocket, int fromId)
        {
            // Recieve invite Id to decline
            int inviteTarget = int.Parse(ByteParser.ConvertToASCII(responseBytes));
            Console.WriteLine("Recieved decline invite request to: " + inviteTarget);
            ServerManager.instance.server.DeclineInvite(inviteTarget);
        }
    }
}