using System;
using System.Collections.Generic;

namespace SNetwork
{
    [Serializable]
    public class MasterNetworkPlayer
    {
        public List<KeyValuePairs> data = new List<KeyValuePairs>();

        public int id = -1;
        public string playfabId = "";
        public string username = "";

        public MasterNetworkPlayer(int id, string username)
        {
            this.id = id;
            this.username = username;
        }

        public MasterNetworkPlayer(int id)
        {
            this.id = id;
            username = "";
        }

        public MasterNetworkPlayer()
        {
            id = -1;
            username = "";
        }
    }
}