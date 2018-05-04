using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SNetwork
{
    [Serializable]
	public class PartyNetworkPlayer
	{

        public int id = -1;
        public string username = "";
        public string playfabId = "";
        public bool PartyUser = false;

        public List<KeyValuePairs> data = new List<KeyValuePairs>();

        public PartyNetworkPlayer(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

	    public void SetPartyUser(bool PartyUser)
	    {
	        this.PartyUser = PartyUser;
	    }

        public PartyNetworkPlayer(int id)
        {
            this.id = id;
            this.username = "";
        }

        public PartyNetworkPlayer()
        {
            this.id = -1;
            this.username = "";
        }
	}
}
