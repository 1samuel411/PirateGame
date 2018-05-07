using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SNetwork.Server
{
    public class Server
    {
        private byte[] _buffer = new byte[50000];
        private int _bufferSize;

        public bool _opened;

        private Thread _userSyncThread;
        private float _userSyncTime;
        public Dictionary<Socket, MasterNetworkPlayer> clientSockets = new Dictionary<Socket, MasterNetworkPlayer>();

        public int maxUsers;
        public List<KeyValuePairs> serverData = new List<KeyValuePairs>();

        public List<Room> rooms = new List<Room>();

        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void BeginAccepting()
        {
            serverSocket.BeginAccept(AcceptedConnection, null);
        }

        private void BeginReceiving(Socket socket)
        {
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetupServer(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000,
            float userSyncTime = 0.5f, int maxUsers = 2000, string serverName = "")
        {
            _userSyncTime = userSyncTime;
            _bufferSize = bufferSize;
            this.maxUsers = maxUsers;
            _buffer = new byte[_bufferSize];
            Console.WriteLine("[SNetworking] Creating and seting up the server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(10);
            BeginAccepting();
            Console.WriteLine("[SNetworking] Success!");

            _opened = true;

            _userSyncThread = new Thread(Sync);
            _userSyncThread.Start();
        }

        private List<int> uniqueIdsToSync = new List<int>();
        private void Sync()
        {
            var iteration = 0;
            while (_opened)
            {
                iteration++;

                Thread.Sleep((int) (_userSyncTime * 1000)/3);

                SetServerData(new KeyValuePairs("UserCount", clientSockets.Count));

                //if(serverData.Count > 0)
                Messaging.instance.SendServerData(ByteParser.ConvertKeyValuePairsToData(serverData.ToArray()),
                    clientSockets, 2);

                Thread.Sleep((int)(_userSyncTime * 1000) / 3);

                // Send User Id
                for (int i = 0; i < uniqueIdsToSync.Count; i++)
                {
                    Messaging.instance.SendId(uniqueIdsToSync[i], uniqueIdsToSync[i], 0, 0, clientSockets);
                    uniqueIdsToSync.RemoveAt(i);
                }


                Thread.Sleep((int) (_userSyncTime * 1000) /3);

                // TODO: Optimize this
                // Send Room data
                for (int i = 0; i < clientSockets.Count; i++)
                {
                    Room room = rooms.First(x => x.usersInRoomIds.Contains(clientSockets.Values.ElementAt(i).id));
                    if (room != null)
                    {
                        // Send room 
                        Messaging.instance.SendRoom(room, clientSockets.Values.ElementAt(i).id, clientSockets);
                    }
                    else
                    {
                        
                    }
                }
            }
        }

        public void CloseServer()
        {
            for (var i = 0; i < clientSockets.Count; i++)
            {
                var clientSocket = clientSockets.Keys.ElementAt(i);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            clientSockets.Clear();

            serverSocket.Close();

            _opened = false;

            Console.WriteLine("[SNetworking] Server successfully closed!");
        }

        private void AcceptedConnection(IAsyncResult AR)
        {
            var socket = serverSocket.EndAccept(AR);
            if (clientSockets.Count >= maxUsers)
            {
                Console.WriteLine("[SNetworking] Max clients reached");
                Messaging.instance.SendInfoMessage(socket, "Full", 0);
                RemoveSocket(socket);
                return;
            }

            Console.WriteLine("[SNetworking] Connection received from: " + socket.LocalEndPoint);

            var uniqueId = new Random().Next(3, maxUsers + 5);

            Console.WriteLine("Checking unique id: " + uniqueId);

            if (clientSockets.Count > 0)
            {
                var unique = false;
                var changed = false;
                while (!unique)
                {
                    changed = false;
                    // TODO: Make this more optimized
                    foreach (var x in clientSockets.Values)
                        if (x.id == uniqueId)
                        {
                            changed = true;
                            uniqueId = new Random().Next(3, maxUsers + 5);
                            Console.WriteLine("Checking unique id: " + uniqueId);
                        }
                    if (!changed)
                        unique = true;
                }
            }

            Console.WriteLine("Assigning unique id: " + uniqueId);
            clientSockets.Add(socket, new MasterNetworkPlayer(uniqueId));
            uniqueIdsToSync.Add(uniqueId);

            string roomId = CreateRoom(uniqueId);
            Console.WriteLine("Assigning Room: " + roomId);

            BeginReceiving(socket);
            BeginAccepting();
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            var socket = (Socket) AR.AsyncState;
            if (!IsConnectedServer(socket))
                return;

            var received = socket.EndReceive(AR);

            var dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);

            if (dataBuffer.Length == 1)
            {
                Console.WriteLine("Connected?");
                return;
            }
            var headerCode = Convert.ToInt32(dataBuffer[0]);
            var customCode = new byte[2];
            customCode[0] = dataBuffer[3];
            customCode[1] = dataBuffer[4];
            var sendCode = Convert.ToInt32(dataBuffer[1]);
            if (headerCode == 0)
                Messaging.instance.Send(dataBuffer.Skip(5).ToArray(), headerCode, sendCode, clientSockets[socket].id,
                    BitConverter.ToInt16(customCode, 0), clientSockets);
            else
                ResponseManager.instance.HandleResponse(dataBuffer.Skip(5).ToArray(), headerCode, sendCode,
                    BitConverter.ToInt16(customCode, 0), socket, clientSockets[socket].id);

            if(socket.Connected)
                BeginReceiving(socket);
        }

        public void SetServerData(KeyValuePairs data)
        {
            serverData = SetData(serverData, data);
        }

        public void SetUserData(int target, KeyValuePairs data)
        {
            for (var i = 0; i < clientSockets.Count; i++)
                if (clientSockets.Values.ElementAt(i).id == target)
                {
                    clientSockets.Values.ElementAt(i).data = SetData(clientSockets.Values.ElementAt(i).data, data);
                    return;
                }
        }

        private List<KeyValuePairs> SetData(List<KeyValuePairs> source, KeyValuePairs data)
        {
            for (var i = 0; i < source.Count; i++)
                if (source[i].Key == data.Key)
                {
                    source[i] = data;
                    return source;
                }
            source.Add(data);
            return source;
        }

        public bool IsConnectedServer(Socket socket)
        {
            var isConnected = Network.IsConnected(socket);
            if (!isConnected)
            {
                var socketRetrieved = clientSockets.FirstOrDefault(t => t.Key == socket);
                Console.WriteLine("[SNetworking] MasterNetworkPlayer: " + socketRetrieved.Value.id +
                                  ", has been lost.");
                LeaveRoom(socketRetrieved.Key, true);
                RemoveSocket(socket);
            }
            return isConnected;
        }

        public void RemoveSocket(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            LeaveRoom(socket, true);
            clientSockets.Remove(socket);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            var socket = (Socket) AR.AsyncState;
            socket.EndSend(AR);
        }

        public string CreateRoom(int userId)
        {
            Room newRoom = new Room(userId);
            rooms.Add(newRoom);
            return newRoom.roomId;
        }

        public void JoinRoom(Socket clientSocket, string id)
        {
            Room room = rooms.FirstOrDefault(x => x.roomId == id);
            if (room == null)
            {
                // TODO: No room found, send message
                return;
            }
            
            room.usersInRoomIds.Add(clientSockets[clientSocket].id);
        }

        public void LeaveRoom(Socket clientSocket, bool forever = false)
        {
            Room room = rooms.FirstOrDefault(x => x.roomId == clientSockets[clientSocket].roomId);
            if (room == null)
            {
                // No room found, CreateRoom
                if (forever)
                    return;

                CreateRoom(clientSockets[clientSocket].id);
                return;
            }

            room.usersInRoomIds.Remove(clientSockets[clientSocket].id);

            if (room.usersInRoomIds.Count <= 0)
            {
                rooms.Remove(room);
            }
        }
    }
}