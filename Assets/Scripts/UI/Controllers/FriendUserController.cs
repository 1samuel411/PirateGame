using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateGame.Managers;
using PirateGame.UI.Views;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using SNetwork.Client;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class FriendUserController : MonoBehaviour
    {

        public FriendUserView friendView;

        public string playfabId;
        public string username;
        public string online;
        public List<string> tags = new List<string>();
        public Invite inviteData;

        private string lastOnlineValue;

        void Awake()
        {
            InvokeRepeating("RefreshInvites", 0.5f, 0.5f);
            Update();
        }

        void Update()
        {
            friendView.onlineIndicator.SetActive(false);
            friendView.offlineIndicator.SetActive(false);
            friendView.unacceptedIndicator.SetActive(false);

            friendView.inviteButton.gameObject.SetActive(false);
            friendView.acceptButton.gameObject.SetActive(false);
            friendView.declineButton.gameObject.SetActive(false);

            friendView.removeFriendButton.gameObject.SetActive(true);

            friendView.usernameText.text = username;

            if (tags.Count <= 0)
                return;

            if (tags.Contains("Friend"))
            {
                switch (online)
                {
                    case "false":
                        friendView.offlineIndicator.SetActive(true);
                        friendView.serverText.text = "Logged off";
                        break;
                    case "NA":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.serverText.text = "North American Server";
                        break;
                    case "SA":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.serverText.text = "South American Servers";
                        break;
                    case "AS":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.serverText.text = "Asian Servers";
                        break;
                    case "EU":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.serverText.text = "European Servers";
                        break;
                    case "AU":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.serverText.text = "Australian Servers";
                        break;
                }
            }
            else
            {
                friendView.unacceptedIndicator.SetActive(true);
                friendView.serverText.text = "Pending";
            }
        }

        public void RefreshPlayfab()
        {
            // Get online status
            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = playfabId;
            request.Keys = new List<string> { "LoggedIn" };
            PlayFabClientAPI.GetUserData(request, GetUserDataSucceed, PlayFabError);
        }

        public void RefreshInvites()
        {
            // Get invites
            for (int i = 0; i < PlayerManager.instance.invites.Length; i++)
            {
                // Invite to me
                if (PlayerManager.instance.invites[i].userTo == MasterClientManager.instance.getId())
                {

                }

                // Invite sent
                if (PlayerManager.instance.invites[i].userFrom == MasterClientManager.instance.getId())
                {

                }
            }
        }

        void PlayFabError(PlayFabError error)
        {
            Debug.Log("[FriendUserController] PlayFab Error: " + error.ErrorMessage);
        }

        void GetUserDataSucceed(GetUserDataResult result)
        {
            online = PlayerManager.instance.GetValue("LoggedIn", "false", result.Data, false);

            if (lastOnlineValue != online)
            {
                lastOnlineValue = online;

                // Set sibling index 1st if online is true
                if (online.Equals("false") == false)
                {
                    transform.SetAsFirstSibling();
                }
                // Set sibling index last if online is false
                else
                {
                    transform.SetAsLastSibling();
                }
            }
        }

        public void AcceptRequest()
        {
            UIManager.instance.loading = true;
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
            request.FunctionName = "acceptFriend";
            request.FunctionParameter = new { targetUser = playfabId };
            request.GeneratePlayStreamEvent = true;
            PlayFabClientAPI.ExecuteCloudScript(request, AcceptRequestResponse, AcceptRequestError);
        }
        
        void AcceptRequestResponse(ExecuteCloudScriptResult result)
        {
            for (int i = 0; i < result.Logs.Count; i++)
            {
                Debug.Log(result.Logs[i].Level + ", " + result.Logs[i].Message);
            }
            Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object messageValue;
            jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
            Debug.Log((string)messageValue);

            tags.Remove("Requestee");
            UIManager.instance.loading = false;
        }

        void AcceptRequestError(PlayFabError error)
        {
            UIManager.instance.loading = false;
            Debug.Log(error.GenerateErrorReport());
        }

        public void RemoveFriend()
        {
            UIManager.instance.loading = true;
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
            request.FunctionName = "removeFriend";
            request.FunctionParameter = new { targetUser = playfabId };
            request.GeneratePlayStreamEvent = true;
            PlayFabClientAPI.ExecuteCloudScript(request, RemoveFriendResponse, RemoveFriendError);
        }

        void RemoveFriendResponse(ExecuteCloudScriptResult result)
        {
            Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object messageValue;
            jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
            Debug.Log((string)messageValue);

            Destroy(gameObject);
            UIManager.instance.loading = false;
        }

        void RemoveFriendError(PlayFabError error)
        {
            UIManager.instance.loading = false;
            Debug.Log(error.GenerateErrorReport());
        }

        public void JoinParty()
        {
            UIManager.instance.LoadMasterServerCall();
            Debug.Log("Accepting invite: " + inviteData.id);
            MasterClientManager.instance.SendInviteAccept(inviteData.id);
        }

        public void InviteParty()
        {
            UIManager.instance.LoadMasterServerCall();
            Debug.Log("Inviting: " + playfabId);
            MasterClientManager.instance.SendInvite(playfabId);
        }

        public void DeclineParty ()
        {
            UIManager.instance.LoadMasterServerCall();
            Debug.Log("Declining invite: " + inviteData.id);
            MasterClientManager.instance.SendInviteDecline(inviteData.id);
        }

    }
}
