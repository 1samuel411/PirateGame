using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork.Client
{
    [Serializable]
    public class Invite
    {

        public DateTime timeSent;
        public int id;
        public string userFrom;
        public string userTo;

    }
}