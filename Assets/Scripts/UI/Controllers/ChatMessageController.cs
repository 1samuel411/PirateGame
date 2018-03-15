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
			chatView.chatText.text = chat.datePosted.ToString("hh:mm:ss") + "   " + chat.playerName + ": " + chat.message;
		}

	}
}
