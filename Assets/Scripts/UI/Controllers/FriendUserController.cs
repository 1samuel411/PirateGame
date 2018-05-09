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
        public Invite[] inviteData;
        public Invite acceptedInvite;

        void Awake()
        {
            Update();
        }

        void Update()
        {
            friendView.usernameText.text = username;

            friendView.onlineIndicator.SetActive(online != "false");

            friendView.inviteButtonGameObject.SetActive(false);

            if (online == "false")
                friendView.serverText.text = "";
            else
            {
                friendView.inviteButtonGameObject.SetActive(true);
                friendView.serverText.text = online + " Server";
            }

            friendView.acceptRequestHolder.SetActive(false);
            friendView.requestedHolder.SetActive(false);
            friendView.friendsHolder.SetActive(false);
            friendView.joinHolder.SetActive(false);
            friendView.inLobbyHolder.gameObject.SetActive(false);

            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].Contains("Requestee"))
                {
                    friendView.acceptRequestHolder.SetActive(true);
                    return;
                }

                if (tags[i].Contains("Requester"))
                {
                    friendView.requestedHolder.SetActive(true);
                    return;
                }
            }

            if (online != PlayerManager.instance.region)
            {
                return;
            }

            friendView.friendsHolder.SetActive(true);

            for (int i = 0; i < inviteData.Length; i++)
            {
                if (inviteData[i].userFrom == MasterClientManager.instance.getId())
                {
                    // We sent one to this friend
                    friendView.friendsHolder.SetActive(false);
                    friendView.joinHolder.SetActive(false);
                    friendView.acceptRequestHolder.gameObject.SetActive(false);
                    friendView.inLobbyHolder.gameObject.SetActive(false);
                    friendView.requestedHolder.gameObject.SetActive(true);

                    acceptedInvite = inviteData[i];
                    return;
                }

                if (inviteData[i].userTo == MasterClientManager.instance.getId())
                {
                    // We recieved one from this friend
                    friendView.acceptRequestHolder.SetActive(false);
                    friendView.requestedHolder.SetActive(false);
                    friendView.friendsHolder.SetActive(false);
                    friendView.inLobbyHolder.gameObject.SetActive(false);
                    friendView.joinHolder.gameObject.SetActive(true);

                    acceptedInvite = inviteData[i];
                }
            }

            for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
            {
                if (playfabId == PlayerManager.instance.roomInfo.usersInRoom[i].playfabId)
                {
                    // Playfab Ids match, they're in the room
                    friendView.acceptRequestHolder.SetActive(false);
                    friendView.requestedHolder.SetActive(false);
                    friendView.friendsHolder.SetActive(false);
                    friendView.joinHolder.SetActive(false);
                    friendView.inLobbyHolder.gameObject.SetActive(true);
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
            Debug.Log("Accepting invite: " + acceptedInvite.id);
            MasterClientManager.instance.SendInviteAccept(acceptedInvite.id);
        }

        public void InviteParty()
        {
            Debug.Log("Inviting: " + playfabId);
            MasterClientManager.instance.SendInvite(playfabId);
        }

        public void DeclineParty ()
        {
            Debug.Log("Declining invite: " + acceptedInvite.id);
            MasterClientManager.instance.SendInviteDecline(acceptedInvite.id);
        }

    }
}
