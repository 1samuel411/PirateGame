using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.UI.Views;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
	public class ChatController : Controller 
	{
		public ChatView chatView;
		
		
		
	}

	public class Chat
	{
		public string playerName;
		public string message;
		public DateTime datePosted;
	}
}
