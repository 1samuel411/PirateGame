using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PirateGame.UI.Views
{
    
	[System.Serializable]
	public class ChatView : View 
	{
		public InputField inputField;

		public Transform holder;

		public GameObject chatPrefab;
		
	}
}
