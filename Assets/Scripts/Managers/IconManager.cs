using System.Collections;
using System.Collections.Generic;
using PirateGame;
using UnityEngine;

namespace PirateGame.Managers
{
    public class IconManager : Base
    {

        public static IconManager instance;

        public Sprite[] rankSprites;

        public Color[] crewColors;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;
        }

    }
}