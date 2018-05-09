using System.Collections.Generic;
using System.Linq;
using PirateGame.Entity;
using PirateGame.Networking;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
using SNetwork;
using SNetwork.Client;
using UnityEngine;

namespace PirateGame.Managers
{
    public class PlayerManager : Base 
    {

        public static PlayerManager instance;

        public bool loggedIn;

        public PirateGame.Entity.EntityPlayer playerEntity;
        public PlayablePlayer playablePlayer;

        public float refreshUserDataTime = 30f;
        private float _refreshUSerDataTimer;

        public User user;
        public List<FriendInfo> friends = new List<FriendInfo>();
        public List<FriendData> friendsData = new List<FriendData>();
        public string region;

        public Room roomInfo
        {
            get
            {
                if (loggedIn) return MasterClientManager.instance.GetRoom();
                else return null;
            }
        }

        public Invite[] invites
        {
            get
            {
                if (loggedIn) return MasterClientManager.instance.GetInvites();
                else return null;
            }
        }

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
            // Ignore master server
            if (Input.GetKeyDown(KeyCode.P))
            {
                masterServerNeeded = false;
            }

            RefreshFriends();

            // Refresh Data
            if (Time.time >= _refreshUSerDataTimer && UIManager.instance.IsScreenOpen("Account") == false)
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

        private float lastRefresh;
        public delegate void RefreshFriendsDelegate();
        public RefreshFriendsDelegate refreshFriendsDelegate;
        public void RefreshFriends()
        {
            if (Time.time >= lastRefresh && UIManager.instance.IsScreenOpen("Account") == false)
            {
                lastRefresh = Time.time + 5f;

                // Refresh friends
                GetFriendsListRequest request = new GetFriendsListRequest();
                PlayFabClientAPI.GetFriendsList(request, GetFriendsRequest, PlayfabError);
            }
        }

        void GetFriendsRequest(GetFriendsListResult result)
        {
            friends = result.Friends;
            // Get Friends Tags
            List<string> keysRequestList = new List<string>();
            keysRequestList.Add("PlayFabId");
            keysRequestList.Add("LoggedIn");

            friendsData.Clear();
            for (int i = 0; i < friends.Count; i++)
            {
                GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
                getUserDataRequest.PlayFabId = friends[i].FriendPlayFabId;
                //getUserDataRequest.Keys = keysRequestList;
                PlayFabClientAPI.GetUserData(getUserDataRequest, GetFriendUserDataResponse, PlayfabError);
            }
        }

        void GetFriendUserDataResponse(GetUserDataResult result)
        {
            for (int i = 0; i < friends.Count; i++)
            {
                if (result.Data.ContainsKey("PlayFabId"))
                {
                    if (friends[i].FriendPlayFabId == result.Data["PlayFabId"].Value)
                    {
                        FriendData data = new FriendData();
                        data.loggedIn = result.Data["LoggedIn"].Value;
                        data.playerId = result.Data["PlayFabId"].Value;
                        friendsData.Add(data);

                        if (refreshFriendsDelegate != null)
                            refreshFriendsDelegate.Invoke();

                        return;
                    }
                }
                else
                {

                }
            }
        }


        void RefreshUserData()
        {
            if (user.playfabId.IsNullOrWhitespace())
                return;

            List<string> keysRequestList = new List<string>();
            keysRequestList.Add("XP");

            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = user.playfabId;
            request.Keys = keysRequestList;
            PlayFabClientAPI.GetUserData(request, GetUserDataResponse, PlayfabError);
        }

        public void PlayerLogin(LoginResult result)
        {
            user.username = result.InfoResultPayload.PlayerProfile.DisplayName;
            user.playfabId = result.PlayFabId;

            UpdateUserDataRequest request = new UpdateUserDataRequest();
            request.Data = new Dictionary<string, string>() { { "PlayFabId", result.PlayFabId } };
            request.Permission = UserDataPermission.Public;
            PlayFabClientAPI.UpdateUserData(request, x => { Debug.Log("Successfully updated playfab Id"); }, PlayfabError);

            UIManager.instance.loading = true;
            RefreshUserData();
        }

        void GetUserDataResponse(GetUserDataResult response)
        {
            if (UIManager.instance.IsScreenOpen("Account"))
            {
                UIManager.instance.loading = false;
                
                // Connect to master server
                UIManager.instance.loading = true;
                Debug.Log("Connecting to Master Server");
                if (masterServerNeeded == false)
                {
                    ResponseMessage fake = new ResponseMessage();
                    fake.type = ResponseMessage.ResponseType.Success;
                    OnConnectToClientInit(fake);
                }
                else
                {
                    MasterClientManager.instance.onConnectDelegate += OnConnectToClientInit;
                    MasterClientManager.instance.onDisconnectDelegate += OnDisconnectFromMasterServer;
                    MasterClientManager.instance.onCloseDelegate += OnCloseFromMasterServer;
                    MasterClientManager.instance.Connect();
                }
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

        public bool masterServerNeeded = true;
        void OnConnectToClientInit(ResponseMessage response)
        {
            UIManager.instance.loading = false;

            if (response.type == ResponseMessage.ResponseType.Failure)
            {
                Debug.Log("Could not connect to master server");    
            }
            else if (response.type == ResponseMessage.ResponseType.Full)
            {
                Debug.Log("Master Server is full");
            }
            else
            {
                Debug.Log("Connected to Master Server");
                UIManager.instance.ScreenSwitch("Menu");
                loggedIn = true;

                MasterClientManager.instance.onConnectDelegate -= OnConnectToClientInit;

                MasterNetworkPlayer masterNetworkPlayer = new MasterNetworkPlayer();
                masterNetworkPlayer.username = user.username;
                masterNetworkPlayer.playfabId = user.playfabId;
                masterNetworkPlayer.roomId = "";

                MasterClientManager.instance.SendNetworkUser(masterNetworkPlayer);

                lastRefresh = Time.time;
            }
        }

        void OnDisconnectFromMasterServer()
        {
            UIManager.instance.loading = false;
        }

        void OnCloseFromMasterServer()
        {
            UIManager.instance.loading = false;

            Debug.Log("Connection to Master Server lost...");
            UIManager.instance.ScreenSwitch("Account");
            loggedIn = false;
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

    [System.Serializable]
    public class FriendData
    {
        public string loggedIn;
        public string playerId;
    }
}