﻿using UnityEngine;
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

        private void Start()
        {
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
            mainMenuView.playButtonText.text = "Not connected";

            if (PlayerManager.instance.roomInfo != null)
            {
                mainMenuView.roomInfoText.text = "Room: " + PlayerManager.instance.roomInfo.roomId;
                for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                {
                    mainMenuView.roomInfoText.text += "\nUser " + (i + 1) + ": " + PlayerManager.instance.roomInfo.usersInRoom[i].username;
                }

                if (PlayerManager.instance.roomInfo.usersInRoom[0].playfabId == PlayerManager.instance.user.playfabId)
                {
                    mainMenuView.playButton.interactable = true;
                    mainMenuView.playButtonText.text = "Play";
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
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 2)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 3)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
                mainMenuView.character3.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId, refresh);
            }
            if (PlayerManager.instance.roomInfo.usersInRoom.Count == 4)
            {
                mainMenuView.character1.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId, refresh);
                mainMenuView.character2.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId, refresh);
                mainMenuView.character3.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId, refresh);
                mainMenuView.character4.character.gameObject.SetActive(true);
                GetCharacterData(mainMenuView.character4, PlayerManager.instance.roomInfo.usersInRoom[3].playfabId, refresh);
            }
        }

        public void GetCharacterData(PirateGame.UI.Views.MainMenuView.MainMenuCharacter character, string playfabData, bool refresh)
        {
            GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest();
            request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
            request.InfoRequestParameters.GetUserData = true;
            request.InfoRequestParameters.UserDataKeys = new List<string>() { "Character", "XP", "PlayMode" };
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
            character.playModeImage.sprite = IconManager.instance.playModeSprites[int.Parse(result.InfoResultPayload.UserData["PlayMode"].Value)];
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
            PlayerManager.instance.SetValue("PlayMode", selection.ToString());

            //if (PlayerManager.instance.roomInfo == null)
                return;

            for(int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
            {
                if(PlayerManager.instance.roomInfo.usersInRoom[i].playfabId == PlayerManager.instance.user.playfabId)
                {
                    if(i == 0)
                        mainMenuView.character1.playModeImage.sprite = IconManager.instance.playModeSprites[selection];
                    else if (i == 1)
                        mainMenuView.character2.playModeImage.sprite = IconManager.instance.playModeSprites[selection];
                    else if (i == 2)
                        mainMenuView.character3.playModeImage.sprite = IconManager.instance.playModeSprites[selection];
                    else if (i == 3)
                        mainMenuView.character4.playModeImage.sprite = IconManager.instance.playModeSprites[selection];

                    break;
                }
            }
        }
    }
}