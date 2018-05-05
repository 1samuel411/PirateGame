using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class FriendsController : Controller
    {

        public FriendsView friendsView;

        void Start()
        {
            
        }

        void Update()
        {

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