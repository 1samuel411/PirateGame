using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;
using SNetwork;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PirateGame.UI.Controllers
{
    public class MainMenuController : Controller
    {
        
        public MainMenuView mainMenuView;

        public string playMenuController;

        public void Play()
        {
            UIManager.instance.ScreenSwitch(playMenuController);
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

        public Room lastRoomInfo = new Room();
        void Update()
        {
            mainMenuView.leaveButton.gameObject.SetActive(false);
            if (PlayerManager.instance.roomInfo != null)
            {
                mainMenuView.roomInfoText.text = "Room: " + PlayerManager.instance.roomInfo.roomId;
                for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                {
                    mainMenuView.roomInfoText.text += "\nUser " + (i + 1) + ": " + PlayerManager.instance.roomInfo.usersInRoom[i].username;
                }

                if(PlayerManager.instance.roomInfo.usersInRoom.Count > 1)
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
                    if (PlayerManager.instance.roomInfo.usersInRoom.Count == 1)
                    {
                        mainMenuView.character1.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId);
                    }
                    if (PlayerManager.instance.roomInfo.usersInRoom.Count == 2)
                    {
                        mainMenuView.character1.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId);
                        mainMenuView.character2.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId);
                    }
                    if (PlayerManager.instance.roomInfo.usersInRoom.Count == 3)
                    {
                        mainMenuView.character1.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId);
                        mainMenuView.character2.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId);
                        mainMenuView.character3.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId);
                    }
                    if (PlayerManager.instance.roomInfo.usersInRoom.Count == 4)
                    {
                        mainMenuView.character1.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character1, PlayerManager.instance.roomInfo.usersInRoom[0].playfabId);
                        mainMenuView.character2.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character2, PlayerManager.instance.roomInfo.usersInRoom[1].playfabId);
                        mainMenuView.character3.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character3, PlayerManager.instance.roomInfo.usersInRoom[2].playfabId);
                        mainMenuView.character4.character.gameObject.SetActive(true);
                        GetCharacterData(mainMenuView.character4, PlayerManager.instance.roomInfo.usersInRoom[3].playfabId);
                    }
                }
            }
            else
            {
                mainMenuView.roomInfoText.text = "Room not found";
            }
        }

        public void GetCharacterData(PirateGame.UI.Views.MainMenuView.MainMenuCharacter character, string playfabData)
        {
            GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest();
            request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
            request.InfoRequestParameters.GetUserData = true;
            request.InfoRequestParameters.UserDataKeys = new List<string>() { "Character", "XP" };
            request.InfoRequestParameters.GetUserAccountInfo = true;
            request.PlayFabId = playfabData;
            PlayFabClientAPI.GetPlayerCombinedInfo(request, x => { GetUserDataResponse(x, character); }, PlayFabError);
        }

        public void GetUserDataResponse(GetPlayerCombinedInfoResult result, PirateGame.UI.Views.MainMenuView.MainMenuCharacter character)
        {
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

        }

    }
}