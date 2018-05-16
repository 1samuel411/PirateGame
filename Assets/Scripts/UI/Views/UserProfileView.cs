using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [System.Serializable]
    public class UserProfileView : View
    {

        public Text profileNameText;
        public Text rankText;

        public Image rankImage;

        public Image loggedInStatus;
        public Image loggedOffStatus;
        public Image loggedPendingStatus;

        public Text lastLoggedIn;
        public Text serverText;

        public GameObject addFriendHolder;

    }
}