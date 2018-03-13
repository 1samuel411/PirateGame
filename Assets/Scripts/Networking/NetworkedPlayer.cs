using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PirateGame.Managers;
using Rewired.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Networking
{
    public class NetworkedPlayer : NetworkingBase
    {

        [SyncVar] public int networkId = 0;

        void Start()
        {
            if (isLocalPlayer)
            {
                if(PNetworkManager.instance.connectAction != null)
                    PNetworkManager.instance.connectAction.Invoke(null);

                SendData();
            }

        }

        void SendData()
        {
            Byte[] myData = ObjectSerializer.Serialize(PlayerManager.instance.user);
            myData = ObjectSerializer.Compress(myData);
            CmdSendData(myData);
        }

        [Command]
        public void CmdSendData(Byte[] data)
        {
            data = ObjectSerializer.Decompress(data);

            NetworkUser user = ServerManager.instance.networkUsers[networkId];
            user.userData = (User) ObjectSerializer.DeSerialize(data);
            ServerManager.instance.networkUsers[networkId] = user;

            ServerManager.instance.RefreshUsers();
        }

        public void ReadyToggle()
        {
            Debug.Log("[Networked Player] Toggling ready");

            if (ServerManager.instance.inLobby == false)
                return;

            if (!isServer)
                UpdateReady();

            CmdReadyToggle();
        }

        void UpdateReady()
        {
            NetworkUser user = ServerManager.instance.networkUsers[networkId];
            user.ready = !user.ready;
            ServerManager.instance.networkUsers[networkId] = user;
        }

        [Command]
        public void CmdReadyToggle()
        {
            UpdateReady();

            ServerManager.instance.RefreshUsers();
        }

        public void ChangeCrew()
        {
            Debug.Log("[Networked Player] Changing Crews");

            if (ServerManager.instance.inLobby == false)
                return;

            CmdChangeCrew();
        }

        void UpdateCrew()
        {
            NetworkUser user = ServerManager.instance.networkUsers[networkId];

            // Get next crew
            int nextCrew = CrewManager.GetNextCrew(user.crew);

            // Check the logistics
            int maxCheck = 0;
            while (ServerManager.instance.crews[nextCrew].members.Count >= 4)
            {
                if (maxCheck > 40)
                    break;

                maxCheck++;
                nextCrew = CrewManager.GetNextCrew(user.crew);
            }

            // TODO: Implement a password check

            // Leave cur Crew
            if (user.crew > -1)
            {
                if (ServerManager.instance.crews[user.crew].leader == user.networkConnection)
                {
                    if (ServerManager.instance.crews[user.crew].members.Count <= 1)
                        ServerManager.instance.crews[user.crew].leader = -1;
                    else
                        ServerManager.instance.crews[user.crew].leader =
                            ServerManager.instance.crews[user.crew].members[1];
                }
                ServerManager.instance.crews[user.crew].members.Remove(user.networkConnection);
            }

            // Update Network Users
            user.crew = nextCrew;
            ServerManager.instance.networkUsers[networkId] = user;

            // Add to crew
            ServerManager.instance.crews[user.crew].members.Add(user.networkConnection);

            // Check leader
            if(ServerManager.instance.crews[user.crew].leader == -1)
                ServerManager.instance.crews[user.crew].leader = user.networkConnection;

        }

        [Command]
        public void CmdChangeCrew()
        {
            UpdateCrew();

            ServerManager.instance.RefreshUsers();
            ServerManager.instance.RefreshCrews();
        }

    }
}
