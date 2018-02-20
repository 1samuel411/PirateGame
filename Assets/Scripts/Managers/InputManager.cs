using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public Rewired.Player player;

        void Awake()
        {
            instance = this;

            player = Rewired.ReInput.players.GetPlayer(0);
        }

        void Update()
        {

        }
    }
}
