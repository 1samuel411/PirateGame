using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PirateGame.Entity;
using PirateGame.Networking;
using PirateGame.UI.Controllers;
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
            if (Input.GetKeyDown(KeyCode.F8))
            {
                masterServerNeeded = false;
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                PNetworkManager.instance.networkAddress = "127.0.0.1";
                PNetworkManager.instance.networkPort = 1500;
                PNetworkManager.instance.PStartHost();
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                PNetworkManager.instance.networkAddress = "127.0.0.1";
                PNetworkManager.instance.networkPort = 1500;
                PNetworkManager.instance.PStartServer();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                PNetworkManager.instance.networkAddress = "127.0.0.1";
                PNetworkManager.instance.networkPort = 1500;
                PNetworkManager.instance.PStartClient();
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
        public void RefreshFriends(bool immediate = false)
        {
            if (immediate)
                lastRefresh = 0;

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
            if(refreshFriendsDelegate != null)
                refreshFriendsDelegate.Invoke();
        }

        public void RefreshUserData()
        {
            if (user.playfabId.IsNullOrWhitespace())
                return;

            List<string> keysRequestList = new List<string>();
            keysRequestList.Add("XP");
            keysRequestList.Add("Coins");
            keysRequestList.Add("Character");
            keysRequestList.Add("Elo");

            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = user.playfabId;
            request.Keys = keysRequestList;
            PlayFabClientAPI.GetUserData(request, GetUserDataResponse, PlayfabError);
        }

        public void PlayerLogin(LoginResult result)
        {
            user.username = result.InfoResultPayload.PlayerProfile.DisplayName;
            user.playfabId = result.PlayFabId;

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
            user.coins = int.Parse(GetValue("Coins", "0", response.Data));
            user.SetCharacterSettings(GetValue("Character", Character.Character.GetDefaultCharacter(), response.Data));
            user.elo = int.Parse(GetValue("Elo", "1000", response.Data));
            user.playMode = 0;
        }

        public string GetValue(string id, string defaultValue , Dictionary<string, UserDataRecord> data, bool sendData = true )
        {
            if (data.ContainsKey(id))
                return data[id].Value;
            else
            {
                if (sendData == false)
                    return defaultValue;

                SetValue(id, defaultValue);
                return defaultValue;
            }
        }

        public void SetValue(string id, string data)
        {
            UpdateUserDataRequest request = new UpdateUserDataRequest();
            request.Data = new Dictionary<string, string>() { { id, data } };
            request.Permission = UserDataPermission.Public;
            PlayFabClientAPI.UpdateUserData(request, result => { Debug.Log("Set Data: " + id + ", with: " + data); }, PlayfabError);
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
                masterNetworkPlayer.elo = user.elo;
                masterNetworkPlayer.xp = user.xp;
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
            UIManager.instance.ScreenSwitch("Account", true);
            loggedIn = false;
        }

        public static int GetRank(int xp)
        {
            int rank = (int)(xp / 200.0f);
            rank = Mathf.Clamp(rank, 0, IconManager.instance.rankSprites.Length);
            return rank + 1;
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
                return PlayerManager.GetRank(xp);
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
        public int coins;
        public int playMode;
        public int elo;

        public Character.CharacterSettings character;

        public void SetCharacterSettings(string settings)
        {
            character = JsonConvert.DeserializeObject<Character.CharacterSettings>(settings);
        }
        public float GetCharacterSetting(string name)
        {
            return character.bodySelections.FirstOrDefault(x => x.name == name).value;
        }
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