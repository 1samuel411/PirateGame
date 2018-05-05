using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class FriendUserController : MonoBehaviour
    {

        public FriendUserView friendView;

        public string playfabId;
        public string username;
        public bool invited;
        public bool request;
        public bool online;

        void Update()
        {
            friendView.usernameText.text = username;

            friendView.onlineIndicator.SetActive(online);

            if (request)
            {
                friendView.invitedHolder.SetActive(false);
                friendView.acceptedHolder.SetActive(false);
                friendView.unacceptedHolder.SetActive(true);
            }
            else if (invited)
            {
                friendView.unacceptedHolder.SetActive(false);
                friendView.acceptedHolder.SetActive(false);
                friendView.invitedHolder.SetActive(true);
            }
            else
            {
                friendView.acceptedHolder.SetActive(true);
                friendView.unacceptedHolder.SetActive(false);
                friendView.invitedHolder.SetActive(false);
            }
        }

        public void AcceptRequest()
        {
            
        }

        public void JoinParty()
        {
            
        }

        public void InviteParty()
        {
            
        }

    }
}
