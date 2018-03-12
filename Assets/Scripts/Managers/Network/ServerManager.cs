using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PirateGame.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Managers
{
    public class ServerManager : NetworkingBase
    {

        public static ServerManager instance;

        public Dictionary<int, NetworkUser> networkUsers = new Dictionary<int, NetworkUser>();
        public List<NetworkUser> networkUser = new List<NetworkUser>();

        public List<Crew> crews = new List<Crew>();

        [SyncVar] public bool inLobby = true;
        [SyncVar] public bool readyToPlay = true;
        [SyncVar] public int lobbyTimer;

        public int lobbyTime = 5;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;
        }

        void Start()
        {
            PNetworkManager.instance.networkUserChange += RefreshUsers;
        }

        void Update()
        {
            networkUser = networkUsers.Values.ToList();

            UpdateLobby();
        }

        public void RefreshCrews()
        {
            Debug.Log("[Refreshing Crews]");
            // only send if server
            if (isServer)
            {
                List<Crew> newCrews = crews;
                for (int i = 0; i < newCrews.Count; i++)
                {
                    newCrews[i].crewPassword = "";
                }
                byte[] data = ObjectSerializer.Serialize(newCrews);
                data = ObjectSerializer.Compress(data);
                RpcRecieveCrews(data);

                Debug.Log("[Server Manager] Sending Crew Data");
            }
        }

        [ClientRpc]
        public void RpcRecieveCrews(byte[] uncompressedData)
        {
            Debug.Log("[Server Manager] Recieving Crew Data");
            uncompressedData = ObjectSerializer.Decompress(uncompressedData);
            crews = ObjectSerializer.DeSerialize(uncompressedData) as List<Crew>;

            if (PNetworkManager.instance.crewsChange != null && !isServer)
                PNetworkManager.instance.crewsChange.Invoke();
        }

        public void RefreshUsers()
        {
            Debug.Log("[Refreshing Users]");
            // Only send if server
            if (isServer)
            {
                byte[] data = ObjectSerializer.Serialize(networkUsers);
                data = ObjectSerializer.Compress(data);
                RpcRecieveUsers(data);
                Debug.Log("[Server Manager] Sending User data");

                CheckTimer();
            }
        }

        [ClientRpc]
        public void RpcRecieveUsers(byte[] uncompressedData)
        {
            Debug.Log("[Server Manager] Recieving User data");
            uncompressedData = ObjectSerializer.Decompress(uncompressedData);
            networkUsers = ObjectSerializer.DeSerialize(uncompressedData) as Dictionary<int, NetworkUser>;

            if (PNetworkManager.instance.networkUserChange != null && !isServer)
                PNetworkManager.instance.networkUserChange.Invoke();
        }

        public void ReadyToggle()
        {
            PNetworkManager.instance.client.connection.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>().ReadyToggle();
        }

        public void ChangeCrew()
        {
            PNetworkManager.instance.client.connection.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>().ChangeCrew();
        }

        void UpdateLobby()
        {
            if (!isServer)
                return;

        }

        private IEnumerator lobbyCoroutine;
        void CheckTimer()
        {
            readyToPlay = true;
            foreach (NetworkUser networkUser in networkUsers.Values)
            {
                if (networkUser.ready == false || networkUser.crew < 0)
                    readyToPlay = false;
            }

            if (readyToPlay == false)
            {
                if (lobbyCoroutine != null)
                {
                    StopCoroutine(lobbyCoroutine);
                    lobbyCoroutine = null;
                }
                return;
            }

            // Everyone is ready! Lets play
            if (lobbyCoroutine == null)
            {
                lobbyTimer = lobbyTime;
                lobbyCoroutine = CountDown();
                StartCoroutine(lobbyCoroutine);
            }
        }

        IEnumerator CountDown()
        {
            while (lobbyTimer > 0)
            {
                yield return new WaitForSeconds(1);
                lobbyTimer -= 1;
            }

            PNetworkManager.instance.ServerChangeScene("Test");
        }
    }
}