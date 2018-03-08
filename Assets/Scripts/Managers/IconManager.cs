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

        void Awake()
        {
            instance = this;
        }

    }
}