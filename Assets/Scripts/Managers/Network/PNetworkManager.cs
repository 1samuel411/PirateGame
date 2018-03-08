using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PirateGame.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Managers
{
	public class PNetworkManager : NetworkManager
	{

	    public static PNetworkManager instance;

	    public ConnectionConfig config;
        public int maxConnections = 40;

        public Dictionary<int, NetworkUser> networkUsers = new Dictionary<int, NetworkUser>();
        public List<NetworkUser> networkUser = new List<NetworkUser>();
	    public Action networkUserChange;

	    public Action<NetworkConnection> connectAction;
	    public Action<NetworkConnection> disconnectAction;

        void Awake()
	    {
	        instance = this;
            DontDestroyOnLoad(gameObject);
        }

	    void Update()
	    {
	        networkUser = networkUsers.Values.ToList();
	    }

        public void PStartHost()
        {
            StartHost(config, maxConnections);
        }

        public void PStartClient()
        {
            StartClient();
        }

	    public void Disconnect()
	    {
	        StopClient();
            StopServer();

            networkUsers.Clear();

            if (networkUserChange != null)
	            networkUserChange.Invoke();


	        if (disconnectAction != null)
	            disconnectAction.Invoke(null);
        }

	    public override void OnClientConnect(NetworkConnection con)
	    {
	        base.OnClientConnect(con);

	        if (connectAction != null)
	        {
	            connectAction.Invoke(con);
	        }

	        if (con.connectionId == 0)
	        {
	            NetworkUser networkUser = new NetworkUser();
	            networkUser.ready = false;
	            networkUser.userData = new User();

	            networkUser.networkConnection = con;
	            networkUser.networkPlayer = new NetworkedPlayer();

	            networkUsers.Add(networkUsers.Count + 1, networkUser);
	        }

	        if (networkUserChange != null)
	            networkUserChange.Invoke();
	    }

	    public override void OnServerConnect(NetworkConnection con)
	    {
	        base.OnServerConnect(con);

	        if (con.connectionId != 0)
	        {
	            NetworkUser networkUser = new NetworkUser();
	            networkUser.ready = false;
	            networkUser.userData = new User();

	            networkUser.networkConnection = con;
	            networkUser.networkPlayer = new NetworkedPlayer();

	            networkUsers.Add(networkUsers.Count + 1, networkUser);
	        }
	    }

	    public override void OnClientDisconnect(NetworkConnection con)
	    {
	        base.OnClientDisconnect(con);

            Debug.Log("------------------------------------------------------2");

            foreach (KeyValuePair<int, NetworkUser> player in networkUsers)
	        {
	            if (player.Value.networkConnection == con)
	            {
	                networkUsers.Remove(player.Key);

	                if (networkUserChange != null)
	                    networkUserChange.Invoke();

	                break;
	            }
	        }

	        if (disconnectAction != null)
	        {
	            disconnectAction.Invoke(con);
	        }
        }

        public override void OnServerDisconnect(NetworkConnection con)
	    {
            base.OnServerDisconnect(con);
	        Debug.Log("------------------------------------------------------1");

            foreach (KeyValuePair<int, NetworkUser> player in networkUsers)
	        {
	            if (player.Value.networkConnection == con)
	            {
	                networkUsers.Remove(player.Key);

	                if (networkUserChange != null)
	                    networkUserChange.Invoke();

	                break;
	            }
	        }

	        if (disconnectAction != null)
	        {
	            disconnectAction.Invoke(con);
	        }
        }
	}

    [System.Serializable]
    public struct NetworkUser
    {
        public NetworkConnection networkConnection;
        public NetworkedPlayer networkPlayer;

        public User userData;

        public bool ready;
    }
}
