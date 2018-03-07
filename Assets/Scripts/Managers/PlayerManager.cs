using UnityEngine;

namespace PirateGame.Managers
{
    public class PlayerManager : Base 
    {

        public static PlayerManager instance;

        public PirateGame.Entity.EntityPlayer playerEntity;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            CheckUI();
        }

        void CheckUI()
        {

        }

    }
}