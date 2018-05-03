using System.Collections.Generic;
using PirateGame.Entity;
using PirateGame.Networking;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
using UnityEngine;

namespace PirateGame.Managers
{
    public class PlayerManager : Base 
    {

        public static PlayerManager instance;

        public PirateGame.Entity.EntityPlayer playerEntity;
        public PlayablePlayer playablePlayer;

        public float refreshUserDataTime = 30f;
        private float _refreshUSerDataTimer;

        public User user;
        //public LoginResult playfabData;

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
            // Refresh Data
            if (Time.time >= _refreshUSerDataTimer)
            {
                _refreshUSerDataTimer = Time.time + refreshUserDataTime;
                RefreshUserData();
            }

            if (playerEntity == null)
            {
                if(PNetworkManager.instance && PNetworkManager.instance.IsClientConnected() && PNetworkManager.instance.client.connection.playerControllers.Count >= 2)
                    playerEntity = PNetworkManager.instance.client.connection.playerControllers[1].gameObject.GetComponent<EntityPlayer>();
                return;
            }

            if (playablePlayer == null && playerEntity != null)
                playablePlayer = playerEntity.GetComponent<PlayablePlayer>();
        }

        public void PlayerLogin(LoginResult result)
        {
            user.username = result.InfoResultPayload.PlayerProfile.DisplayName;
            user.playfabId = result.PlayFabId;

            UIManager.instance.loading = true;
            RefreshUserData();
        }

        public void RefreshUserData()
        {
            Debug.Log("[PlayerManager] Refreshing our User's Data");

            if (user.playfabId.IsNullOrWhitespace())
                return;

            List<string> keysRequestList = new List<string>();
            keysRequestList.Add("XP");

            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = user.playfabId;
            request.Keys = keysRequestList;
            PlayFabClientAPI.GetUserData(request, GetUserDataResponse, PlayfabError);
        }

        void GetUserDataResponse(GetUserDataResult response)
        {
            if (UIManager.instance.IsScreenOpen("Account"))
            {
                UIManager.instance.loading = false;
                UIManager.instance.ScreenSwitch("Menu");
            }

            // Update XP
            user.xp = int.Parse(GetValue("XP", "0", response.Data));
        }

        public string GetValue(string id, string defaultValue, Dictionary<string, UserDataRecord> data)
        {
            if (data.ContainsKey(id))
                return data[id].Value;
            else
            {
                UpdateUserDataRequest request = new UpdateUserDataRequest();
                request.Data = new Dictionary<string, string>() { { id, defaultValue } };
                request.Permission = UserDataPermission.Public;
                PlayFabClientAPI.UpdateUserData(request, result => { Debug.Log("Reset Key: " + id + ", with: " + defaultValue); }, PlayfabError);
                return defaultValue;
            }
        }

        void PlayfabError(PlayFabError error)
        {
            Debug.Log("[PlayerManager] PlayFab Error: " + error.ErrorMessage);
        }
    }

    [System.Serializable]
    public class User
    {
        public string playfabId;

        public string username;

        public int rank
        {
            get
            {
                int rank = (int)(xp / 200.0f);
                rank = Mathf.Clamp(rank, 0, IconManager.instance.rankSprites.Length);
                return rank + 1;
            }
        }

        public int xpToRank
        {
            get { return (rank * 200); }
        }

        public int xpToLevel
        {
            get
            {
                if (rank >= 1)
                    return ((rank - 1) * 200);
                else
                    return 0;
            }
        }
        public int xp;
    }

    [System.Serializable]
    public class PrivateUser
    {
        public string username;
        public string playerId;
        public string email;
        public string secret;
    }
}