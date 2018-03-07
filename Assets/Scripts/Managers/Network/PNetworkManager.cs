using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.Managers
{
	public class PNetworkManager : NetworkManager
	{

	    public static PNetworkManager instance;

	    public ConnectionConfig config;
	    public int maxConnections = 40; 

        void Awake()
	    {
	        instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PStartServer()
        {
            StartServer(config, maxConnections);
        }

        public void PStartClient()
        {
            
        }
    }
}
