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

        public MainMenuCharacter character1;
        public MainMenuCharacter character2;
        public MainMenuCharacter character3;
        public MainMenuCharacter character4;

        [System.Serializable]
        public class MainMenuCharacter
        {
            public Character.Character character;
            public Text nameText;
            public Image rankImage;
            public GameObject hoverParticle;
        }
    }
}
