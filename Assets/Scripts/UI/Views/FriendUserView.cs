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
        public Text usernameText;
        public Text serverText;

        public Button inviteButton;
        public Button acceptButton;
        public Button declineButton;
        public Button removeFriendButton;

        public GameObject onlineIndicator;
        public GameObject offlineIndicator;
        public GameObject unacceptedIndicator;
    }
}
