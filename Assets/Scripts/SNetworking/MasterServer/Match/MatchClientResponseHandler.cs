using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SNetwork.Client
{
    public class MatchClientResponseHandler
    {
        private MatchClient _client;

        public MatchClientResponseHandler(MatchClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            ResponseManager.instance.Clear();
        }
    }
}
