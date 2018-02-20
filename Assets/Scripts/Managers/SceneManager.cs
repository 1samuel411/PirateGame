using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class SceneManager : MonoBehaviour
    {

        public static SceneManager instance;

        void Awake()
        {
            instance = this;
        }
    }
}