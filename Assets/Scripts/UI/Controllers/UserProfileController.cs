using PirateGame.Managers;
using PirateGame.UI.Views;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class UserProfileController : Controller
    {

        public UserProfileView profileView;

        public string playFabId;

        public DateTime lastLoggedIn;
        public string loggedIn;

        void Init()
        {
            UpdateUI();

            // Get User
            GetUser();
        }

        public void GetUser()
        {
            GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest();
            request.PlayFabId = playFabId;
            request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
            request.InfoRequestParameters.GetUserAccountInfo = true;
            request.InfoRequestParameters.GetUserData = true;
            request.InfoRequestParameters.UserDataKeys = new List<string> { "XP", "LoggedIn", "LastLogin" };
            PlayFabClientAPI.GetPlayerCombinedInfo(request, GetUserResult, GetUserFail);
        }

        void GetUserResult(GetPlayerCombinedInfoResult result)
        {
            if (result.InfoResultPayload.UserData.ContainsKey("LastLogin"))
                lastLoggedIn = DateTime.Parse(result.InfoResultPayload.UserData["LastLogin"].Value);
            if (result.InfoResultPayload.UserData.ContainsKey("LoggedIn"))
                loggedIn = result.InfoResultPayload.UserData["LoggedIn"].Value;

            int xp = 0;
            if (result.InfoResultPayload.UserData.ContainsKey("XP"))
                xp = int.Parse(result.InfoResultPayload.UserData["XP"].Value);
            int rank = PlayerManager.GetRank(xp);
            profileView.rankText.text = rank.ToString();
            profileView.rankImage.sprite = IconManager.instance.rankSprites[rank];
            profileView.profileNameText.text = result.InfoResultPayload.AccountInfo.Username;
        }

        void GetUserFail(PlayFabError e)
        {
            Debug.Log("Error: " + e.ErrorMessage);
        }

        private bool addedFriend = false;
        private void Update()
        {
            FriendInfo friend = PlayerManager.instance.friends.FirstOrDefault(x => x.FriendPlayFabId == playFabId);

            profileView.loggedInStatus.gameObject.SetActive(false);
            profileView.loggedOffStatus.gameObject.SetActive(false);
            profileView.loggedPendingStatus.gameObject.SetActive(false);

            profileView.serverText.text = "";
            profileView.lastLoggedIn.text = "Unknown";;
            if (friend != null)
            {
                // Exists
                profileView.addFriendHolder.SetActive(false);

                if (friend.Tags.Contains("Friend"))
                {
                    // Friends
                    if (loggedIn == "false")
                    {
                        profileView.lastLoggedIn.text = GetRelativeTime.Get(lastLoggedIn);
                        profileView.loggedOffStatus.gameObject.SetActive(true);
                    }
                    else
                    {
                        profileView.lastLoggedIn.text = "Now";
                        profileView.loggedInStatus.gameObject.SetActive(true);

                    }
                    if (loggedIn == "NA")
                        profileView.serverText.text = "North America";
                    else if (loggedIn == "SA")
                        profileView.serverText.text = "South America";
                    else if (loggedIn == "AS")
                        profileView.serverText.text = "Asia";
                    else if (loggedIn == "EU")
                        profileView.serverText.text = "Europe";
                    else if (loggedIn == "AS")
                        profileView.serverText.text = "Australia";


                }
                else
                {
                    profileView.loggedPendingStatus.gameObject.SetActive(true);
                }
            }
            else
            {
                // Doesn't exist
                if(addedFriend == false)
                    profileView.addFriendHolder.SetActive(true);
                profileView.loggedPendingStatus.gameObject.SetActive(true);
            }
        }

        void UpdateUI()
        {

        }

        public void ShowMe(string playFabId)
        {
            this.playFabId = playFabId;
            Init();
        }

        public void AddFriend()
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
            request.FunctionName = "addFriend";
            request.FunctionParameter = new { targetUser = playFabId };
            request.GeneratePlayStreamEvent = true;
            PlayFabClientAPI.ExecuteCloudScript(request, AddFriend, AddFriendError);
        }

        public void AddFriend(ExecuteCloudScriptResult result)
        {
            Debug.Log("Added friend: " + playFabId);
            addedFriend = true;
        }

        public void AddFriendError(PlayFabError e)
        {
            Debug.Log("Error encountered adding friend");
        }

        public void LocalKill()
        {
            UserProfileController.Kill();
        }

        private static GameObject profileShow;
        public static void Show(string playFabId)
        {
            if (profileShow != null)
                Destroy(profileShow);
            GameObject newProfile = Instantiate(Resources.Load("UserProfile")) as GameObject;
            profileShow = newProfile;
            newProfile.GetComponent<UserProfileController>().ShowMe(playFabId);
        }

        public static void Kill()
        {
            Destroy(profileShow);
        }

        public static bool Exists()
        {
            return profileShow != null;
        }

    }
}