using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class SpawnableObjectInfo : Base
    {

        public bool ready = false;

        void OnEnable()
        {
            ready = false;
        }

        void OnDisable()
        {
            ready = true;
        }
    }
}
