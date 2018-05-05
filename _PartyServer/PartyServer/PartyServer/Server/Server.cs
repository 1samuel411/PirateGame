using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using SNetwork;

namespace SNetwork.Server
{
    public class Server
    {

        private byte[] _buffer = new byte[50000];
	    private int _bufferSize;
        public Dictionary<Socket, MasterNetworkPlayer> clientSockets = new Dictionary<Socket, MasterNetworkPlayer>();
        public List<KeyValuePairs> serverData = new List<KeyValuePairs>();

        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public int maxUsers;
	    private float _userSyncTime;

	    public bool _opened;

        private void BeginAccepting()
        {
            serverSocket.BeginAccept(new AsyncCallback(AcceptedConnection), null);
        }

        private void BeginReceiving(Socket socket)
        {
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (ObjectDisposedException e)
            {
				Console.WriteLine(e);
            }
        }

        private Thread _userSyncThread;
        public void SetupServer(string ip = "127.0.0.1", int port = 100, int bufferSize = 50000, float userSyncTime = 0.5f, int maxUsers = 2, string serverName = "")
        {
	        this._userSyncTime = userSyncTime;
	        this._bufferSize = bufferSize;
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

        private void Sync()
        {
            int iteration = 0;
            while (_opened)
            {
                iteration++;

                // TODO: Make this update when a user leaves or something
                Thread.Sleep((int)(_userSyncTime * 1000));

				SetServerData(new KeyValuePairs("UserCount", clientSockets.Count));
				
				Messaging.instance.SendServerData(ByteParser.ConvertKeyValuePairsToData(serverData.ToArray()), clientSockets, 2);
            }
        }
        
        public void CloseServer()
	    {
		    for (int i = 0; i < clientSockets.Count; i++)
		    {
			    Socket clientSocket = clientSockets.Keys.ElementAt(i);
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
            Socket socket = serverSocket.EndAccept(AR);
            if (clientSockets.Count >= maxUsers)
            {
                Console.WriteLine("[SNetworking] Max clients reached");
                Messaging.instance.SendInfoMessage(socket, "Full",0);
                return;
            }
            Console.WriteLine("[SNetworking] Connection received from: " + socket.LocalEndPoint);

            int uniqueId = new Random().Next(3, 9999999);

            Console.WriteLine("Checking unique id: " + uniqueId);

            if (clientSockets.Count > 0)
            {
                bool unique = false;
                bool changed = false;
                while (!unique)
                {
                    changed = false;
					// TODO: Make this more optimized
                    foreach (MasterNetworkPlayer x in clientSockets.Values)
                    {
                        if (x.id == uniqueId)
                        {
                            changed = true;
                            uniqueId = new Random().Next(3, 9999999);
                            Console.WriteLine("Checking unique id: " + uniqueId);
                        }
                    }
                    if (!changed)
                        unique = true;
                }
            }

            Console.WriteLine("Assigning unique id: " + uniqueId);

            clientSockets.Add(socket, new MasterNetworkPlayer(uniqueId));

            Messaging.instance.SendId(uniqueId, uniqueId, 0, 0, clientSockets);

            BeginReceiving(socket);
            BeginAccepting();
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            if (!IsConnectedServer(socket))
                return;

            int received = socket.EndReceive(AR);

            byte[] dataBuffer = new byte[received];
            Array.Copy(_buffer, dataBuffer, received);

            int headerCode = Convert.ToInt32(dataBuffer[0]);
            byte[] customCode = new byte[2];
            customCode[0] = dataBuffer[3];
            customCode[1] = dataBuffer[4];
            int sendCode = Convert.ToInt32(dataBuffer[1]);
            if (headerCode == 0)
            {
                // custom message, reroute it
                Messaging.instance.Send(dataBuffer.Skip(5).ToArray(), headerCode, sendCode, clientSockets[socket].id, BitConverter.ToInt16(customCode, 0), clientSockets);
            }
            else
            {
                // message with a header, keep it
                ResponseManager.instance.HandleResponse(dataBuffer.Skip(5).ToArray(), headerCode, sendCode, BitConverter.ToInt16(customCode, 0), socket, clientSockets[socket].id);
            }

            if(socket.Connected)
				BeginReceiving(socket);
        }

        public void SetServerData(KeyValuePairs data)
        {
            serverData = SetData(serverData, data);
        }

        public void SetUserData(int target, KeyValuePairs data)
        {
            for (int i = 0; i < clientSockets.Count; i++)
            {
                if (clientSockets.Values.ElementAt(i).id == target)
                {
                    clientSockets.Values.ElementAt(i).data = SetData(clientSockets.Values.ElementAt(i).data, data);
                    return;
                }
            }
        }

        private List<KeyValuePairs> SetData(List<KeyValuePairs> source, KeyValuePairs data)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Key == data.Key)
                {
                    source[i] = data;
                    return source;
                }
            }
            source.Add(data);
            return source;
        }

        public bool IsConnectedServer(Socket socket)
        {
            bool isConnected = Network.IsConnected(socket);
            if (!isConnected)
            {
                KeyValuePair<Socket, MasterNetworkPlayer> socketRetrieved = clientSockets.FirstOrDefault(t => t.Key == socket);
                Console.WriteLine("[SNetworking] MasterNetworkPlayer: " + socketRetrieved.Value.id + ", has been lost.");
                RemoveSocket(socket);
            }
            return isConnected;
        }

        public void RemoveSocket(Socket socket)
        {
            clientSockets.Remove(socket);
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
