using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PirateGame.Managers;
using PirateGame.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Managers
{
	public class PNetworkManager : NetworkManager
	{

	    public static PNetworkManager instance;

	    public Action networkUserChange;

        public Action<NetworkConnection> serverStartAction;
        public Action<NetworkConnection> connectAction;
        public Action<NetworkConnection> disconnectAction;

	    public GameObject playerGameObject;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

	        instance = this;
            DontDestroyOnLoad(gameObject);

        }

        public void PStartHost()
        {
            if (ServerManager.instance != null)
            {
                ServerManager.instance.networkUsers.Clear();
            }

            StartHost();
            UIManager.instance.loading = true;
        }

        public void PStartServer()
        {
            if (ServerManager.instance != null)
            {
                ServerManager.instance.networkUsers.Clear();
            }
            StartServer();
            Debug.Log("Starting Server");
            UIManager.instance.loading = true;
        }

        public void PStartClient()
        {
            if (ServerManager.instance != null)
            {
                ServerManager.instance.networkUsers.Clear();
            }
            StartClient();
            UIManager.instance.loading = true;
        }

        public void Disconnect()
	    {
	        ServerManager.instance.networkUsers.Clear();

            StopClient();
            StopServer();

	        ServerManager.instance.networkUsers.Clear();

            if (networkUserChange != null)
	            networkUserChange.Invoke();

	        if (disconnectAction != null)
	            disconnectAction.Invoke(null);
        }

	    public override void OnClientConnect(NetworkConnection con)
	    {
	        base.OnClientConnect(con);

            Debug.Log("LOCAL CLIENT - ID JOINED - " + con.connectionId);
            
	        UIManager.instance.loading = false;

            UIManager.instance.ScreenSwitch("LoadScene");
	    }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if(serverStartAction != null)
                serverStartAction.Invoke(null);

            PNetworkManager.instance.ServerChangeScene("Game");
        }

        public override void OnServerConnect(NetworkConnection con)
	    {
	        base.OnServerConnect(con);

	        NetworkUser networkUser = new NetworkUser();
	        networkUser.ready = false;
	        networkUser.userData = new User();

	        networkUser.networkConnection = con.connectionId;

            ServerManager.instance.networkUsers.Add(con.connectionId, networkUser);

            connectAction.Invoke(con);
	    }

	    public override void OnServerAddPlayer(NetworkConnection con, short playerControllerId)
	    {
	        if (con.playerControllers.Count >= 1)
	            return;

	        base.OnServerAddPlayer(con, playerControllerId);

            NetworkedPlayer networkPlayer = con.playerControllers[playerControllerId]
	            .gameObject.GetComponent<NetworkedPlayer>();
	        networkPlayer.networkId = con.connectionId;
        }

        public override void OnServerReady(NetworkConnection con)
	    {
	        base.OnServerReady(con);

	        if (networkUserChange != null)
                networkUserChange.Invoke();

            Debug.Log("Ready");
        }

	    public override void OnClientDisconnect(NetworkConnection con)
	    {
	        base.OnClientDisconnect(con);

            Debug.Log("LOCAL CLIENT - ID LEFT - " + con.connectionId);

            if(con.connectionId == ServerManager.instance.netId.Value)
	            if (disconnectAction != null)
	                disconnectAction.Invoke(con);

	        UIManager.instance.loading = false;
	    }

	    public override void OnServerDisconnect(NetworkConnection con)
	    {
	        base.OnServerDisconnect(con);

	        foreach (KeyValuePair<int, NetworkUser> player in ServerManager.instance.networkUsers)
	        {
	            if (player.Value.networkConnection == con.connectionId)
	            {
	                ServerManager.instance.networkUsers.Remove(player.Key);

	                if (networkUserChange != null)
	                    networkUserChange.Invoke();

	                break;
	            }
	        }

            Destroy(con.playerControllers[0].gameObject);
        }

        public override void ServerChangeScene(string newSceneName)
	    {
	        base.ServerChangeScene(newSceneName);
	    }

	    public override void OnClientSceneChanged(NetworkConnection conn)
	    {
            //if(hosting == false)
	            //base.OnClientSceneChanged(conn);

            Debug.Log("GAME SCENE LOADED");

	        UIManager.instance.FadeOut();

            conn.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>().AddPlayer();
        }
    }
}

[System.Serializable]
public class NetworkUser
{
    public int networkConnection;

    public User userData;

    public bool ready;

    public int crew = -1;

}
