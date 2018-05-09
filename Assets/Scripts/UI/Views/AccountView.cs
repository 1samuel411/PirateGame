﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace PirateGame.UI.Views
{
	[System.Serializable]
	public class AccountView : View 
	{

		public InputField usernameFieldLogin;
		public InputField passwordFieldLogin;
		public InputField usernameFieldRegister;
		public InputField emailFieldRegister;
		public InputField passwordFieldRegister;
		public InputField passwordConfirmFieldRegister;
	    public InputField emailFieldForgotPassword;

	    public Dropdown regionSelectableLogin;
	    public Dropdown regionSelectableRegister;

	    public Toggle rememberMeToggle;

        public GameObject loginViewObject;
		public Text loginViewText;
	    public GameObject registerViewObject;
	    public Text registerViewText;
	    public GameObject forgotPasswordViewObject;
	    public Text forgotPasswordViewText;

        public GameObject passwordInfoBox;
	    public Text passwordInfoBoxText;

    }
}