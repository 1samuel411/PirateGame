using UnityEngine;
using UnityEngine.UI;
using System;

namespace PirateGame.UI
{
    public class Controller : MonoBehaviour
    {
        public Button [] buttons;

        public Action EnableAction;
        public Action DisableAction;

        void OnEnable()
        {
            if(EnableAction != null)
                EnableAction.Invoke();
        }

        void OnDisable()
        {
            if(DisableAction != null)
                DisableAction.Invoke();
        }
    }
}