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
		
		public List<ChatMessageController> chats = new List<ChatMessageController>();
		

		void Awake()
		{
			PNetworkManager.instance.chatChange += RefreshChat;
		}

		public void SendChat(string text)
		{
			ServerManager.instance.SendChat(text);
			chatView.inputField.text = "";
		}

		void RefreshChat()
		{
			int newChat = 0;
			for(int i = 0; i < chats.Count; i++)
			{
				for(int x = 0; x < ServerManager.instance.chats.Count; x++)
				{
					if(chats[i].chat == ServerManager.instance.chats[x])
					{
						newChat++;
					}
				}
			}
			AddChat(ServerManager.instance.chats[newChat]);
		}

		void AddChat(Chat chat)
		{
			GameObject newObj = GameObject.Instantiate(chatView.chatPrefab, Vector3.zero, Quaternion.identity);
			newObj.transform.SetParent(chatView.holder);
			newObj.transform.localPosition = Vector3.zero;
			newObj.transform.localScale = Vector3.one;

			ChatMessageController messageController = newObj.GetComponent<ChatMessageController>();
			messageController.chat = chat;
		} 
	}

	[System.Serializable]
	public class Chat
	{
		public string playerName;
		public string message;
		public DateTime datePosted;
	}
}
