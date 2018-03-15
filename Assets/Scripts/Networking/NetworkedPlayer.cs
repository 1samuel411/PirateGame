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

        public void ChangeCrew(int x)
        {
            Debug.Log("[Networked Player] Changing Crews");

            if (ServerManager.instance.inLobby == false)
                return;

            CmdChangeCrew(x);
        }

        public void LeaveCrew()
        {
            Debug.Log("[Networked Player] Leaving Crews");

            if (ServerManager.instance.inLobby == false)
                return;

            ChangeCrew(-1);
        }

        void UpdateCrew(int x)
        {
            NetworkUser user = ServerManager.instance.networkUsers[networkId];

            // Get next crew
            int nextCrew = x;

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
            
            if(nextCrew < 0)
                return;

            ServerManager.instance.networkUsers[networkId] = user;

            // Add to crew
            ServerManager.instance.crews[user.crew].members.Add(user.networkConnection);

            // Check leader
            if(ServerManager.instance.crews[user.crew].leader == -1)
                ServerManager.instance.crews[user.crew].leader = user.networkConnection;

        }

        [Command]
        public void CmdChangeCrew(int x)
        {
            UpdateCrew(x);

            ServerManager.instance.RefreshUsers();
            ServerManager.instance.RefreshCrews();
        }

        public void KickCrewMember(int target, bool authoritative = false)
        {
            CmdKickCrewMember(target, authoritative);
        }

        [Command]
        public void CmdKickCrewMember(int target, bool authoritative)
        {
            if (!authoritative)
            {
                Crew myCrew = ServerManager.instance.crews[ServerManager.instance.networkUsers[networkId].crew];

                if (myCrew == null)
                    return;

                if (networkId == myCrew.leader)
                {
                    // kick authorized
                    ServerManager.instance.crews[ServerManager.instance.networkUsers[target].crew]
                        .members.Remove(target);
                    ServerManager.instance.networkUsers[target].crew = -1;

                    ServerManager.instance.RefreshUsers();
                    ServerManager.instance.RefreshCrews();
                }
            }
            else
            {
                // kick authorized
                ServerManager.instance.crews[ServerManager.instance.networkUsers[target].crew].members.Remove(target);
                ServerManager.instance.networkUsers[target].crew = -1;

                ServerManager.instance.RefreshUsers();
                ServerManager.instance.RefreshCrews();
            }
        }

        public void ChangeCrewName(string newName)
        {
            Debug.Log("Renaming Crew");
            CmdChangeCrewName(newName);
        }

        [Command]
        public void CmdChangeCrewName(string newName)
        {
            if(networkId == ServerManager.instance.crews[ServerManager.instance.networkUsers[networkId].crew].leader)
                ServerManager.instance.crews[ServerManager.instance.networkUsers[networkId].crew].crewName = newName;

            ServerManager.instance.RefreshUsers();
            ServerManager.instance.RefreshCrews();
        }

    }
}
