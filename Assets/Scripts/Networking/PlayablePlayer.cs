using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Networking
{
    public class PlayablePlayer : Base
    {

        public MonoBehaviour[] localOnlyComponents;

        void Update()
        {
            for (int i = 0; i < localOnlyComponents.Length; i++)
            {
                localOnlyComponents[i].enabled = ServerManager.instance.myNetworkPlayer.isLocalPlayer;
            }
        }

    }
}