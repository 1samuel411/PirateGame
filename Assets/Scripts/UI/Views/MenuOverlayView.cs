using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [System.Serializable]
    public class MenuOverlayView : View
    {

        public Image rankImage;

        public Image xpBarImage;

        public Text usernameText;

        public Text rankText;
        public Text xpText;

        public Text connectedUsersText;

        public GameObject notificationsGameObject;
        public Text notificationsText;

        public Text friendsText;

    }
}