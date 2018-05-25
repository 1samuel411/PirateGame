using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PirateGame.Networking;
using UnityEngine;
using UnityEngine.Networking;
using PirateGame.UI.Controllers;
using UnityEngine.Networking.NetworkSystem;

namespace PirateGame.Managers
{
    public class ServerManager : NetworkingBase
    {

        public static ServerManager instance;

        public Dictionary<int, NetworkUser> networkUsers = new Dictionary<int, NetworkUser>();
        public List<NetworkUser> networkUsersList = new List<NetworkUser>();

        private NetworkedPlayer _myNetworkedPlayer;
        public NetworkedPlayer myNetworkPlayer
        {
            get
            {
                if(_myNetworkedPlayer == null)
                {
                    _myNetworkedPlayer = PNetworkManager.instance.client.connection.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>();
                }
                return _myNetworkedPlayer;
            }
        }

        private int myId = 0;

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
            
        }

        private void Update()
        {
            networkUsersList = networkUsers.Values.ToList();
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
    }
}