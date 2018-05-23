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
        public DateTime lastOnlineTime;
        public List<string> tags = new List<string>();
        public Invite inviteRecieved;
        public Invite inviteSent;

        public bool expanded;
        
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
            friendView.notificationIndicator.SetActive(false);

            friendView.inviteButton.gameObject.SetActive(false);
            friendView.acceptButton.gameObject.SetActive(false);
            friendView.declineButton.gameObject.SetActive(false);
            friendView.joinButton.gameObject.SetActive(false);
            friendView.dontJoinButton.gameObject.SetActive(false);
            friendView.removeFriendButton.gameObject.SetActive(false);

            if(expanded)
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
                        if(lastOnlineTime == new DateTime())
                            friendView.lastOnlineText.text = "Unkown";
                        else
                            friendView.lastOnlineText.text = GetRelativeTime.Get(lastOnlineTime);

                        friendView.serverText.text = "Logged off";
                        transform.SetAsLastSibling();
                        break;
                    case "NA":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.lastOnlineText.text = "Now";
                        friendView.serverText.text = "North American Server";
                        break;
                    case "SA":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.lastOnlineText.text = "Now";
                        friendView.serverText.text = "South American Servers";
                        break;
                    case "AS":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.lastOnlineText.text = "Now";
                        friendView.serverText.text = "Asian Servers";
                        break;
                    case "EU":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.lastOnlineText.text = "Now";
                        friendView.serverText.text = "European Servers";
                        break;
                    case "AU":
                        friendView.onlineIndicator.SetActive(true);
                        friendView.lastOnlineText.text = "Now";
                        friendView.serverText.text = "Australian Servers";
                        break;
                }
                
                if (friendView.onlineIndicator.activeInHierarchy)
                {
                    if (!PlayerManager.instance.roomInfo.usersInRoom.Any(x => x.playfabId == playfabId) && PlayerManager.instance.region == online)
                    {
                        friendView.inviteButton.gameObject.SetActive(true);
                        if (inviteRecieved != null && !string.IsNullOrEmpty(inviteRecieved.userFrom))
                        {
                            // Recieved invite
                            friendView.notificationIndicator.SetActive(true);
                            friendView.joinButton.gameObject.SetActive(true);
                            friendView.dontJoinButton.gameObject.SetActive(true);
                        }

                        if (inviteSent != null && !string.IsNullOrEmpty(inviteSent.userFrom))
                        {
                            // We sent
                            friendView.inviteButton.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                friendView.lastOnlineText.text = "Unkown";
                friendView.unacceptedIndicator.SetActive(true);
                transform.SetAsFirstSibling();

                if (tags.Contains("Requestee"))
                {
                    friendView.serverText.text = "Pending";
                    friendView.acceptButton.gameObject.SetActive(true);
                    friendView.declineButton.gameObject.SetActive(true);
                }
                else
                {
                    friendView.serverText.text = "Friend Request Sent";
                    friendView.unacceptedIndicator.SetActive(true);
                }
            }

            if (expanded == false)
            {
                friendView.inviteButton.gameObject.SetActive(false);
                friendView.acceptButton.gameObject.SetActive(false);
                friendView.declineButton.gameObject.SetActive(false);
                friendView.joinButton.gameObject.SetActive(false);
                friendView.dontJoinButton.gameObject.SetActive(false);
                friendView.removeFriendButton.gameObject.SetActive(false);
            }
        }

        public void Expand()
        {
            if (expanded)
            {
                expanded = false;
                return;
            }
            GetComponentInParent<FriendsController>().ExpandFriend(this);
        }

        public void RefreshPlayfab()
        {
            // Get online status
            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = playfabId;
            request.Keys = new List<string> { "LoggedIn", "LastLogin" };
            PlayFabClientAPI.GetUserData(request, GetUserDataSucceed, PlayFabError);
        }

        public void RefreshInvites()
        {
            inviteRecieved = null;
            inviteSent = null;

            if (PlayerManager.instance.invites == null)
                return;

            // Get invites
            for (int i = 0; i < PlayerManager.instance.invites.Length; i++)
            {
                // Invite to me
                if (PlayerManager.instance.invites[i].userTo == playfabId)
                {
                    inviteSent = PlayerManager.instance.invites[i];
                }

                // Invite sent
                if (PlayerManager.instance.invites[i].userFrom == playfabId)
                {
                    inviteRecieved = PlayerManager.instance.invites[i];
                }
            }
        }

        void PlayFabError(PlayFabError error)
        {
            Debug.Log("[FriendUserController] PlayFab Error: " + error.ErrorMessage);
        }

        private string lastOnlineValue;
        void GetUserDataSucceed(GetUserDataResult result)
        {
            if (gameObject == null)
                return;

            online = PlayerManager.instance.GetValue("LoggedIn", "false", result.Data, false);
            string lastOnlineText = PlayerManager.instance.GetValue("LastLogin", "", result.Data, false);
            if(lastOnlineText == "")
            {
                lastOnlineTime = new DateTime();
            }
            else
            {
                lastOnlineTime = DateTime.Parse(lastOnlineText);
            }

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
            tags.Add("Friend");
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
            PlayerManager.instance.friends.RemoveAll(x => x.FriendPlayFabId == playfabId);
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
            Debug.Log("Accepting invite: " + inviteRecieved.id);
            MasterClientManager.instance.SendInviteAccept(inviteRecieved.id);
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
            Debug.Log("Declining invite: " + inviteRecieved.id);
            MasterClientManager.instance.SendInviteDecline(inviteRecieved.id);
        }

    }
}
