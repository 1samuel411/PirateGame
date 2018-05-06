using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Sirenix.Utilities;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class AddFriendController : Controller
    {

        public AddFriendView addFriendView;

        public string nameToSearch;

        void OnEnable()
        {
            addFriendView.responseText.text = "";
            addFriendView.searchInputField.text = "";
            nameToSearch = "";
        }

        public void UpdateName(string value)
        {
            if (UIManager.instance.loading)
                return;

            nameToSearch = value;
        }

        public void Search()
        {
            if (nameToSearch.IsNullOrWhitespace())
            {
                return;
            }
            // Add friend
            UIManager.instance.loading = true;

            GetAccountInfoRequest request = new GetAccountInfoRequest();
            request.Username = nameToSearch;
            PlayFabClientAPI.GetAccountInfo(request, GetAccountInfo, GetAccountInfoError);
        }

        void GetAccountInfo(GetAccountInfoResult result)
        {
            AddFriend(result.AccountInfo.PlayFabId);
        }

        void GetAccountInfoError(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.AccountNotFound)
                addFriendView.responseText.text = "Error found";

            UIManager.instance.loading = false;
        }

        void AddFriend(string playfabId)
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
            request.FunctionName = "addFriend";
            request.FunctionParameter = new { targetUser = playfabId };
            request.GeneratePlayStreamEvent = true;
            PlayFabClientAPI.ExecuteCloudScript(request, AddFriend, AddFriendError);
        }

        void AddFriend(ExecuteCloudScriptResult result)
        {
            /*
            Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object messageValue;
            jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
            Debug.Log((string)messageValue);
            */

            PlayerManager.instance.RefreshFriends();
            Back();
            UIManager.instance.loading = false;
        }

        void AddFriendError(PlayFabError error)
        {
            UIManager.instance.loading = false;
            Debug.Log(error.GenerateErrorReport());
            addFriendView.responseText.text = error.GenerateErrorReport();
        }

        public void Back()
        {
            UIManager.instance.UnloadScreenAdditive("AddFriend");
        }

    }
}