using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    [Serializable]
    public class MainMenuView : View
    {

        public Text roomInfoText;

        public Button leaveButton;

        public Button playButton;

        public Text matchMakingText;
        public GameObject matchMakingHolder;

        public GameObject matchMakingCancel;

        public Text playButtonText;

        public MainMenuCharacter character1;
        public MainMenuCharacter character2;
        public MainMenuCharacter character3;
        public MainMenuCharacter character4;

        public GameObject taken;

        [System.Serializable]
        public class MainMenuCharacter
        {
            public Character.Character character;
            public Text nameText;
            public Image rankImage;
            public Image playModeImage;
            public GameObject hoverParticle;
            public int playMode = -1;
        }

        public Dropdown playModeDropdown;
    }
}
