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
	    public Action crewsChange;
		public Action chatChange;

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
                ServerManager.instance.crews = ServerManager.instance.defaultCrews;
                ServerManager.instance.networkUsers.Clear();
				ServerManager.instance.chats.Clear();
            }
            StartHost();
            UIManager.instance.loading = true;
        }

        public void PStartClient()
        {
            if (ServerManager.instance != null)
            {
                ServerManager.instance.crews = ServerManager.instance.defaultCrews;
                ServerManager.instance.networkUsers.Clear();
				ServerManager.instance.chats.Clear();
            }
            StartClient();
            UIManager.instance.loading = true;
        }

        public void Disconnect()
	    {
	        ServerManager.instance.crews = ServerManager.instance.defaultCrews;
	        ServerManager.instance.networkUsers.Clear();
	        ServerManager.instance.chats.Clear();

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
	    }

	    public override void OnServerConnect(NetworkConnection con)
	    {
	        base.OnServerConnect(con);

	        NetworkUser networkUser = new NetworkUser();
	        networkUser.ready = false;
	        networkUser.userData = new User();

	        networkUser.networkConnection = con.connectionId;

            ServerManager.instance.networkUsers.Add(con.connectionId, networkUser);
	    }

	    public override void OnServerAddPlayer(NetworkConnection con, short playerControllerId)
	    {
	        if (playerControllerId == 0)
	        {
	            base.OnServerAddPlayer(con, playerControllerId);

                NetworkedPlayer networkPlayer = con.playerControllers[playerControllerId]
	                .gameObject.GetComponent<NetworkedPlayer>();
	            networkPlayer.networkId = con.connectionId;
	        }
            else if (playerControllerId == 1)
	        {
	            GameObject player = (GameObject)Instantiate(playerGameObject, Vector3.zero, Quaternion.identity);
	            NetworkServer.AddPlayerForConnection(con, player, playerControllerId);
                con.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>().CmdSetMyPlayer(con.playerControllers[1].gameObject);
                con.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>().TargetSetMyPlayer(con, con.playerControllers[1].gameObject);
            }
        }

	    public override void OnServerReady(NetworkConnection con)
	    {
	        base.OnServerReady(con);

	        if (networkUserChange != null)
                networkUserChange.Invoke();
        }

	    public override void OnClientDisconnect(NetworkConnection con)
	    {
	        base.OnClientDisconnect(con);

            Debug.Log("LOCAL CLIENT - ID LEFT - " + con.connectionId);
            if(con.connectionId == 0)
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
                    ServerManager.instance.RemoveCrewMember(player.Key);
	                ServerManager.instance.networkUsers.Remove(player.Key);

	                if (networkUserChange != null)
	                    networkUserChange.Invoke();

	                break;
	            }
	        }
	    }

	    public override void ServerChangeScene(string newSceneName)
	    {
	        base.ServerChangeScene(newSceneName);
	    }

	    public override void OnClientSceneChanged(NetworkConnection conn)
	    {
	        base.OnClientSceneChanged(conn);

            Debug.Log("GAME SCENE LOADED");

	        UIManager.instance.ScreenSwitch("Lobby");
	        UIManager.instance.FadeOut();

            ClientScene.AddPlayer(conn, 1);
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
