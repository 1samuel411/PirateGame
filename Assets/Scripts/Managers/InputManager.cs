using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class InputManager : Base
    {
        public static InputManager instance;

        public Rewired.Player player;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

            player = Rewired.ReInput.players.GetPlayer(0);
        }

        void Update()
        {

        }
    }
}
