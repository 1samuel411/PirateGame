using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.UI.Views;
using PirateGame.Managers;
using Sirenix.Utilities;
using UnityEngine.UI;

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
		    if (text.IsNullOrWhitespace())
		        return;

			ServerManager.instance.SendChat(text);
			chatView.inputField.text = "";
		}

		void RefreshChat()
		{
			AddChat(ServerManager.instance.chats[ServerManager.instance.chats.Count-1]);
		}

		void AddChat(Chat chat)
		{
			GameObject newObj = GameObject.Instantiate(chatView.chatPrefab, Vector3.zero, Quaternion.identity);
			newObj.transform.SetParent(chatView.holder);
			newObj.transform.localPosition = Vector3.zero;
			newObj.transform.localScale = Vector3.one;

		    StartCoroutine(EnableForceExpand());

            ChatMessageController messageController = newObj.GetComponent<ChatMessageController>();
			messageController.chat = chat;
		}

	    IEnumerator EnableForceExpand()
	    {
		    chatView.holder.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
	        yield return null;
            chatView.holder.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
        }
    }

	[System.Serializable]
	public class Chat
	{
		public string playerName;
		public string message;
		public DateTime datePosted;
	    public Color playerCrewColor;

        public bool crewOnly;

	}
}
