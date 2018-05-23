using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;
using SNetwork;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace PirateGame.UI.Controllers
{
    public class MainMenuController : Controller
    {
        
        public MainMenuView mainMenuView;

        public string playMenuController;

        public void Play()
        {
            MasterClientManager.instance.SendMatchMake();
        }

        public void CancelMatchmake()
        {
            MasterClientManager.instance.CancelMatchMake();
        }

        public void Leave()
        {
            MasterClientManager.instance.SendLeave();
        }

        void OnMatchFound()
        {
            // Open up UI Cover
            UIManager.instance.loading = true;

            // Connect
            PNetworkManager.instance.networkAddress = MasterClientManager.instance.getMatch().ip;
            PNetworkManager.instance.networkPort = MasterClientManager.instance.getMatch().port;
            PNetworkManager.instance.PStartClient();
        }

        private void Start()
        {
            MasterClientManager.instance.onMatchFound += OnMatchFound;
            mainMenuView.character1.character.gameObject.SetActive(false);
            mainMenuView.character2.character.gameObject.SetActive(false);
            mainMenuView.character3.character.gameObject.SetActive(false);
            mainMenuView.character4.character.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (PlayerManager.instance == null)
                return;

            mainMenuView.playModeDropdown.value = PlayerManager.instance.user.playMode;
        }

        private float refreshTimer = 0;
        public Room lastRoomInfo = new Room();
        void Update()
        {
            mainMenuView.matchMakingHolder.gameObject.SetActive(false);
            mainMenuView.leaveButton.gameObject.SetActive(false);
            mainMenuView.playModeDropdown.gameObject.SetActive(true);
            mainMenuView.playButton.gameObject.SetActive(true);
            mainMenuView.playButton.interactable = false;
            mainMenuView.matchMakingCancel.gameObject.SetActive(false);
            mainMenuView.playButtonText.text = "Not connected";

            if (PlayerManager.instance.roomInfo != null)
            {
                bool taken = false;
                mainMenuView.roomInfoText.text = "Room: " + PlayerManager.instance.roomInfo.roomId;
                for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                {
                    mainMenuView.roomInfoText.text += "\nUser " + (i + 1) + ": " + PlayerManager.instance.roomInfo.usersInRoom[i].username;

                    if (PlayerManager.instance.roomInfo.usersInRoom[i].playfabId != PlayerManager.instance.user.playfabId)
                    {
                        if(GetCharacter(i).playMode == PlayerManager.instance.user.playMode)
                        {

                        }
                    }
                }
                mainMenuView.taken.gameObject.SetActive(taken);

                if (PlayerManager.instance.roomInfo.usersInRoom[0].playfabId == PlayerManager.instance.user.playfabId)
                {
                    bool twoAlike = false;
                    for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                    {
                        for (int x = 0; x < PlayerManager.instance.roomInfo.usersInRoom.Count; x++)
                        {
                            if (GetCharacter(i).playMode == GetCharacter(x).playMode && PlayerManager.instance.roomInfo.usersInRoom[i].playfabId != PlayerManager.instance.roomInfo.usersInRoom[x].playfabId)
                            {
                                twoAlike = true;
                            }
                        }

                    }
                    if (twoAlike)
                    {
                        mainMenuView.playButton.interactable = false;
                        mainMenuView.playButtonText.text = "Play Modes";
                    }
                    else
                    {
                        mainMenuView.playButton.interactable = true;
                        mainMenuView.playButtonText.text = "Play";
                    }
                }
                else
                {
                    mainMenuView.playButtonText.text = "Waiting";
                }

                if (PlayerManager.instance.roomInfo.usersInRoom.Count > 1)
                {
                    mainMenuView.leaveButton.gameObject.SetActive(true);
                }

                if (PlayerManager.instance.roomInfo.roomId != lastRoomInfo.roomId || PlayerManager.instance.roomInfo.usersInRoom.Count != lastRoomInfo.usersInRoom.Count)
                {
                    lastRoomInfo = PlayerManager.instance.roomInfo;
                    mainMenuView.character1.character.gameObject.SetActive(false);
                    mainMenuView.character2.character.gameObject.SetActive(false);
                    mainMenuView.character3.character.gameObject.SetActive(false);
                    mainMenuView.character4.character.gameObject.SetActive(false);
                    // Get Data
                    GetData(true);
                }

                if(Time.time >= refreshTimer)
                {
                    refreshTimer = Time.time + 2f;
                    GetData(false);
                }

                if(PlayerManager.instance.roomInfo.matchmaking)
                {
                    mainMenuView.matchMakingHolder.gameObject.SetActive(true);
                    mainMenuView.leaveButton.gameObject.SetActive(false);
                    mainMenuView.playButton.gameObject.SetActive(false);
                    mainMenuView.playModeDropdown.gameObject.SetActive(false);
                    DateTime newTime = new DateTime((DateTime.UtcNow - PlayerManager.instance.roomInfo.matchmakingBegin).Ticks);
                    mainMenuView.matchMakingText.text = "Waiting\n\n<b>" + newTime.ToString("mm:ss") + "</b>";

                    if (PlayerManager.instance.roomInfo.usersInRoom[0].playfabId == PlayerManager.instance.user.playfabId)
                    {
                        mainMenuView.matchMakingCancel.gameObject.SetActive(true);
                    }
                }
                else if(PlayerManager.instance.roomInfo.inMatch)
                {
                    if (MasterClientManager.instance.getMatch() != null)
                    {
                        // in match
                        mainMenuView.matchMakingHolder.gameObject.SetActive(true);
                        
                        mainMenuView.leaveButton.gameObject.SetActive(false);
                        mainMenuView.playButton.gameObject.SetActive(false);
                        mainMenuView.playModeDropdown.gameObject.SetActive(false);
                        mainMenuView.matchMakingText.text = "Found Match\n\n<b>Connecting...</b>";
                    }
                }
            }
            else
            {
                mainMenuView.roomInfoText.text = "Room not found";
            }
        }

        void GetData(bool refresh)
        {
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 1)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character1.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[0].playMode];
                mainMenuView.character1.playMode = PlayerManager.instance.roomInfo.usersInRoom[0].playMode;
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 2)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character1.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[0].playMode];
                mainMenuView.character1.playMode = PlayerManager.instance.roomInfo.usersInRoom[0].playMode;
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
                mainMenuView.character2.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[1].playMode];
                mainMenuView.character2.playMode = PlayerManager.instance.roomInfo.usersInRoom[1].playMode;
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 3)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character1.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[0].playMode];
                mainMenuView.character1.playMode = PlayerManager.instance.roomInfo.usersInRoom[0].playMode;
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
                mainMenuView.character2.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[1].playMode];
                mainMenuView.character2.playMode = PlayerManager.instance.roomInfo.usersInRoom[1].playMode;
                mainMenuView.character3.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId, refresh);
                mainMenuView.character3.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[2].playMode];
                mainMenuView.character3.playMode = PlayerManager.instance.roomInfo.usersInRoom[2].playMode;
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 4)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character1.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[0].playMode];
                mainMenuView.character1.playMode = PlayerManager.instance.roomInfo.usersInRoom[0].playMode;
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
                mainMenuView.character2.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[1].playMode];
                mainMenuView.character2.playMode = PlayerManager.instance.roomInfo.usersInRoom[1].playMode;
                mainMenuView.character3.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId, refresh);
                mainMenuView.character3.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[2].playMode];
                mainMenuView.character3.playMode = PlayerManager.instance.roomInfo.usersInRoom[2].playMode;
                mainMenuView.character4.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character4, PlayerManager.instance.roomInfo.usersInRoom[3].playfabId, refresh);
                mainMenuView.character4.playModeImage.sprite = IconManager.instance.playModeSprites[PlayerManager.instance.roomInfo.usersInRoom[3].playMode];
                mainMenuView.character4.playMode = PlayerManager.instance.roomInfo.usersInRoom[3].playMode;
            }
        }

        public void GetCharacterData(PirateGame.UI.Views.MainMenuView.MainMenuCharacter character, string playfabData, bool refresh)
        {
            GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest();
            request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
            request.InfoRequestParameters.GetUserData = true;
            request.InfoRequestParameters.UserDataKeys = new List<string>() { "Character", "XP" };
            request.InfoRequestParameters.GetUserAccountInfo = true;
            request.PlayFabId = playfabData;
            PlayFabClientAPI.GetPlayerCombinedInfo(request, x => { GetUserDataResponse(x, character, refresh); }, PlayFabError);
        }

        public void GetUserDataResponse(GetPlayerCombinedInfoResult result, PirateGame.UI.Views.MainMenuView.MainMenuCharacter character, bool refresh)
        {
            if(refresh)
                character.character.SetCharacter(JsonConvert.DeserializeObject<Character.CharacterSettings>(result.InfoResultPayload.UserData["Character"].Value));
            character.nameText.text = result.InfoResultPayload.AccountInfo.Username;
            character.rankImage.sprite = IconManager.instance.rankSprites[PlayerManager.GetRank(int.Parse(result.InfoResultPayload.UserData["XP"].Value))];
        }

        public void PlayFabError(PlayFabError e)
        {
            Debug.Log("Could not get character data!");
        }

        private int hoveringCharacter = -1;
        public void HoverCharacter(int character)
        {
            if (hoveringCharacter != character)
                hoveringCharacter = character;
            else
                return;

            mainMenuView.character1.hoverParticle.gameObject.SetActive(false);
            mainMenuView.character2.hoverParticle.gameObject.SetActive(false);
            mainMenuView.character3.hoverParticle.gameObject.SetActive(false);
            mainMenuView.character4.hoverParticle.gameObject.SetActive(false);

            if (character == 0)
                mainMenuView.character1.hoverParticle.gameObject.SetActive(true);
            if(character == 1)
                mainMenuView.character2.hoverParticle.gameObject.SetActive(true);
            if(character == 2)
                mainMenuView.character3.hoverParticle.gameObject.SetActive(true);
            if(character == 3)
                mainMenuView.character4.hoverParticle.gameObject.SetActive(true);
        }

        public void ClickCharacter(int character)
        {
            // Don't click on our own player
            if (PlayerManager.instance.roomInfo.usersInRoom[character].playfabId == PlayerManager.instance.user.playfabId)
                return;

            UserProfileController.Show(PlayerManager.instance.roomInfo.usersInRoom[character].playfabId);
        }

        // Change playMode
        public void ChangeMode(int selection)
        {
            Debug.Log("Changing mode: " + selection);
            MasterClientManager.instance.SendNewPlayMode(selection);
            return;
        }

        public MainMenuView.MainMenuCharacter GetCharacter(int index)
        {
            if (index == 0)
                return mainMenuView.character1;
            else if (index == 1)
                return mainMenuView.character2;
            else if (index == 2)
                return mainMenuView.character3;
            else if (index == 3)
                return mainMenuView.character4;
            else
                return null;
        }
    }
}