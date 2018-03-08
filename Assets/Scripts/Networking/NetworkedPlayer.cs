using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Networking
{
    public class NetworkedPlayer : NetworkClient
    {

        void Start()
        {
            Application.runInBackground = true;
        }

        void Update()
        {

        }

        public override void Disconnect()
        {
            
        }
    }
}
