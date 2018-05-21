using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PirateGame.Entity;
using PirateGame.Managers;
using Rewired.Data;
using UnityEngine;
using UnityEngine.Networking;
using PirateGame.UI.Controllers;
using UnityEngine.Playables;

namespace PirateGame.Networking
{
    public class NetworkedPlayer : NetworkingBase
    {

        [SyncVar] public int networkId = 0;

        public GameObject player;
        private PlayablePlayer playablePlayer;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            playablePlayer = player.GetComponent<PlayablePlayer>();
            CameraManager.instance.cameraObject.target = playablePlayer.transform;

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

        public void AddPlayer()
        {
            PlayerManager.instance.playerEntity = player.GetComponent<EntityPlayer>();

            player.gameObject.transform.position = Vector3.zero;
            player.gameObject.transform.rotation = Quaternion.identity;
        }
    }
}
