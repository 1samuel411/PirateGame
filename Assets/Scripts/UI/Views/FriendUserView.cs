using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{

    [Serializable]
    public class FriendUserView : View
    {
        public GameObject onlineIndicator;

        public Text usernameText;

        public GameObject invitedHolder;
        public GameObject acceptedHolder;
        public GameObject unacceptedHolder;

    }
}
