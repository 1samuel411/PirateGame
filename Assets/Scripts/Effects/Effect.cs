using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Effects
{
    public class Effect : Base, IEffect
    {

        public float autoDestructTime;

        private float activateTime;

        void OnEnable()
        {
            SetAutoDestructTime();
        }

        void Awake()
        {
            SetAutoDestructTime();
        }

        void Update()
        {
            CheckAutoDestruct();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetAutoDestructTime()
        {
            activateTime = autoDestructTime + Time.time;
        }

        public void CheckAutoDestruct()
        {
            if(Time.time >= activateTime)
                gameObject.SetActive(false);
        }

        public float GetAutoDestructTime()
        {
            return autoDestructTime;
        }
    }
}
