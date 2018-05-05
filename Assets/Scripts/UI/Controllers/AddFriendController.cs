using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class AddFriendController : Controller
    {

        public AddFriendView addFriendView;

        public string nameToSearch;


        public void UpdateName(string value)
        {
            nameToSearch = value;
        }

        public void Search()
        {
            
        }

        public void Back()
        {
            UIManager.instance.UnloadScreenAdditive("AddFriend");
        }

    }
}