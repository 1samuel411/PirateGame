using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PirateGame.UI.Views;

namespace PirateGame.UI.Controllers
{
	public class AccountController : Controller 
	{

		public AccountView accountView;

		public bool login;

		public string email;
		public string username;
		public string password;
		public string passwordConfirm;

		void OnEnable()
		{
			accountView.emailFieldRegister.text = email;

			accountView.usernameFieldLogin.text = username;
			accountView.usernameFieldRegister.text = username;
		
			accountView.passwordFieldLogin.text = password;
			accountView.passwordFieldRegister.text = password;
			accountView.passwordConfirmFieldRegister.text = "";
			passwordConfirm = "";

			SetLoginMessage("Please login");
			SetRegisterMessage("Please enter your information to register");
		}

		void Update()
		{
			accountView.loginViewObject.SetActive(login);
			accountView.registerViewObject.SetActive(!login);
		}

		public void SetLoginMessage(string message)
		{
			accountView.loginViewText.text = message;
		}

		public void SetRegisterMessage(string message)
		{
			accountView.registerViewText.text = message;
		}

		public void SetLogIn(bool value)
		{
			login = value;
		}

		public void UpdateEmail(string value)
		{
			email = value;
		}

		public void UpdateUsername(string value)
		{
			username = value;
		}

		public void UpdatePassword(string value)
		{
			password = value;
		}

		public void UpdatePasswordConfirm(string value)
		{
			passwordConfirm = value;
		}

		public void Login()
		{
			Debug.Log("Logging user in: " + username + ", " + password);
		}

		public void Register()
		{
			// Test password
			if(passwordConfirm != password)
			{
				SetRegisterMessage("Passwords do not match");
				return;
			}

			Debug.Log("Registering user: " + username + ", " + email + ", " + password);
		}

	}
}
