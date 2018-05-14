using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using SNetwork.Client;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class FriendsController : Controller
    {

        public FriendsView friendsView;

        public List<FriendUserController> friendUserController = new List<FriendUserController>();

        void Start()
        {
            PlayerManager.instance.refreshFriendsDelegate += RefreshFriends;
        }

        void OnEnable()
        {
            RespawnFriends();
        }

        void RespawnFriends()
        {
            for (int i = 0; i < friendUserController.Count; i++)
            {
                if (friendUserController[i] != null)
                    GameObject.Destroy(friendUserController[i].gameObject);
            }

            friendUserController.Clear();

            if (PlayerManager.instance.loggedIn == false)
                return;

            for (int i = 0; i < PlayerManager.instance.friends.Count; i++)
            {
                friendUserController.Add(CreateFriendUserController(PlayerManager.instance.friends[i].FriendPlayFabId, PlayerManager.instance.friends[i].TitleDisplayName, PlayerManager.instance.friends[i].Tags));
            }

            RefreshFriends();
        }

        void RefreshFriends()
        {
            for(int i = 0; i < PlayerManager.instance.friends.Count; i++)
            {
                int found = 0;
                for(int x = 0; x < friendUserController.Count; x++)
                {
                    if(PlayerManager.instance.friends[i].FriendPlayFabId == friendUserController[x].playfabId)
                    {
                        found++;
                        break;
                    }
                }

                // New user, create
                if(found <= 0)
                {
                    FriendUserController controller = CreateFriendUserController(PlayerManager.instance.friends[i].FriendPlayFabId, PlayerManager.instance.friends[i].TitleDisplayName, PlayerManager.instance.friends[i].Tags);
                    friendUserController.Add(controller);
                }
            }

            for(int i = 0; i < friendUserController.Count; i++)
            {
                bool found = false;
                for(int x = 0; x < PlayerManager.instance.friends.Count; x++)
                {
                    if(PlayerManager.instance.friends[x].FriendPlayFabId == friendUserController[i].playfabId)
                    {
                        // Load data again
                        found = true;
                        friendUserController[i].playfabId = PlayerManager.instance.friends[x].FriendPlayFabId;
                        friendUserController[i].username = PlayerManager.instance.friends[x].TitleDisplayName;
                        friendUserController[i].tags = PlayerManager.instance.friends[x].Tags;
                        break;
                    }
                }

                if (!found)
                {
                    if (friendUserController[i] != null)
                    {
                        Destroy(friendUserController[i].gameObject);
                        friendUserController.RemoveAt(i);
                    }
                    continue;
                }

                friendUserController[i].RefreshPlayfab();
            }
        }

        public void ExpandFriend(FriendUserController controller)
        {
            for(int i = 0; i < friendUserController.Count; i++)
            {
                friendUserController[i].expanded = false;
            }

            controller.expanded = true;
        }

        FriendUserController CreateFriendUserController(string playfabId, string username, List<string> tags)
        {
            GameObject friendPrefab = Instantiate(friendsView.friendUserPrefab);
            friendPrefab.transform.SetParent(friendsView.friendsListHolder);
            friendPrefab.transform.localPosition = Vector3.zero;
            friendPrefab.transform.localScale = Vector3.one;
            friendPrefab.transform.rotation = Quaternion.identity;

            FriendUserController friendUserController = friendPrefab.GetComponent<FriendUserController>();
            friendUserController.playfabId = playfabId;
            friendUserController.username = username;
            friendUserController.tags = tags;
            return friendUserController;
        }

        public void AddFriends()
        {
            UIManager.instance.AddScreenAdditive("AddFriend");
        }

        public void Back()
        {
            UIManager.instance.UnloadScreenAdditive("Friends");
        }

    }
}