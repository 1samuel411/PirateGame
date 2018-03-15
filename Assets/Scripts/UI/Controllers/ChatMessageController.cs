using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.UI.Views;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
	public class ChatMessageController : Controller 
	{

		public ChatMessageView chatView;
		
		public Chat chat;

		void Start()
		{
			SetData();
		}

		void Update()
		{
			SetData();
		}

		void SetData()
		{
            chatView.chatText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(chat.playerCrewColor) + ">" + chat.datePosted.ToString("h:m:s") + "   " + chat.playerName + ": " + chat.message + "</color>";

		}

	}
}
