using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class SceneManager : Base
    {

        public static SceneManager instance;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }
    }
}