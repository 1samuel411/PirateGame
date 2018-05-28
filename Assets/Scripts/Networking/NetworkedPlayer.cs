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
        private PlayablePlayer _playablePlayer;
        public PlayablePlayer playablePlayer
        {
            get
            {
                if (_playablePlayer == null)
                    _playablePlayer = player.GetComponent<PlayablePlayer>();

                return _playablePlayer;
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (isLocalPlayer)
            {
                if(CameraManager.instance != null)
                    CameraManager.instance.cameraObject.target = playablePlayer.transform;

                if (PNetworkManager.instance.connectAction != null)
                    PNetworkManager.instance.connectAction.Invoke(null);

                SendData();
            }
        }

        private float sendRate = .09f;
        private float curSendTimer = 0;
        private void Update()
        {
            if (Time.time >= curSendTimer && isLocalPlayer)
            {
                curSendTimer = Time.time + sendRate;

                // Sync 
            }
        }

        #region Send User
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
        #endregion

        public void AddPlayer()
        {
            PlayerManager.instance.playerEntity = player.GetComponent<EntityPlayer>();
            player.transform.position = PNetworkManager.instance.GetStartPosition().position;
            player.transform.rotation = PNetworkManager.instance.GetStartPosition().rotation;
        }
    }
}
