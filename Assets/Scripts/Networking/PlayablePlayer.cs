using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Networking
{
    public class PlayablePlayer : NetworkingBase
    {

        [SyncVar] public int playerId;

        public void SendMyData()
        {
            
        }

    }
}