using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [System.Serializable]
    public class LobbyView : View
    {

        public Transform playerHolder;

        public GameObject userLobbyPrefab;

        public Image readyButton;
        public Text readyText;

        public Color readyColor;
        public Color unReadyColor;

        public Text infoText;

        public Image crewButton;
        public Text crewText;

        public GameObject crewUserLobbyPrefab;
        public Transform crewPlayerHolder;
        public Text crewNameText;

    }
}