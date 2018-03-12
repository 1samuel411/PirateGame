using PirateGame.Networking;
using UnityEngine;

namespace PirateGame.Managers
{
    public class PlayerManager : Base 
    {

        public static PlayerManager instance;

        public PirateGame.Entity.EntityPlayer playerEntity;

        public User user;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            user.username = "User" + Random.Range(0, 9999);
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

    [System.Serializable]
    public class User
    {
        public string username;

        public int rank
        {
            get
            {
                int rank = (int)(xp / 20.0f);
                Mathf.Clamp(rank, 0, IconManager.instance.rankSprites.Length);
                return rank + 1;
            }
        }

        public int xpToRank
        {
            get { return (rank * 20); }
        }
        public int xp;

    }
}