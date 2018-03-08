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

        public View view;

        void OnEnable()
        {
            if(EnableAction != null)
                EnableAction.Invoke();

            Enabled();
        }
        public virtual void Enabled() { }

        void OnDisable()
        {
            if(DisableAction != null)
                DisableAction.Invoke();

            Disabled();
        }
        public virtual void Disabled() { }
    }
}