using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PirateGame.Managers;
using UnityEngine;
using PirateGame.UI.Views;
using PlayFab;
using PlayFab.ClientModels;

namespace PirateGame.UI.Controllers
{
	public class AccountController : Controller 
	{

		public AccountView accountView;

		public bool login = true;

		public string email;
		public string username;
		public string password;
		public string passwordConfirm;

	    private float passwordInfoBoxTime;

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

		    if (Time.time >= passwordInfoBoxTime)
		    {
		        accountView.passwordInfoBox.SetActive(false);
		    }
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

	    public bool passwordValid;
	    public void UpdatePasswordCheck()
	    {
	        passwordInfoBoxTime = Time.time + 3f;

            string pass = accountView.passwordFieldRegister.text;
	        string passConfirm = accountView.passwordConfirmFieldRegister.text;
	        // Requirements
            accountView.passwordInfoBox.gameObject.SetActive(true);

	        Regex hasNumber = new Regex(@"[0-9]+");
	        Regex hasUpperChar = new Regex(@"[A-Z]+");
	        Regex hasMinimum8Chars = new Regex(@".{8,}");

            bool passwordMatch = pass == passConfirm;

	        accountView.passwordInfoBoxText.text = hasNumber.IsMatch(pass) == false ? "Contain a number" : UIManager.StrikeThrough("Contain a number");
	        accountView.passwordInfoBoxText.text += "\n";
            accountView.passwordInfoBoxText.text += hasUpperChar.IsMatch(pass) == false ? "Contain an uppercase letter" : UIManager.StrikeThrough("Contain an uppercase letter");
	        accountView.passwordInfoBoxText.text += "\n";
            accountView.passwordInfoBoxText.text += hasMinimum8Chars.IsMatch(pass) == false ? "At least 8 characters" : UIManager.StrikeThrough("At least 8 characters");
	        accountView.passwordInfoBoxText.text += "\n";
            accountView.passwordInfoBoxText.text += passwordMatch == false ? "Passwords must match" : UIManager.StrikeThrough("Passwords must match");

	        if (hasNumber.IsMatch(pass) && hasUpperChar.IsMatch(pass) && hasMinimum8Chars.IsMatch(pass) && passwordMatch)
	        {
	            passwordValid = true;
	        }
	        else
	        {
	            passwordValid = false;
	        }
        }

		public void Login()
		{
		    UIManager.instance.loading = true;

            SetLoginMessage("Logging in");
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
		    request.Username = username;
		    request.Password = password;
		    request.TitleId = SceneManager.instance.playfabTitleId;
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnLoginError);

            Debug.Log("Logging user in: " + username + ", " + password);
		}

	    void OnLoginResult(LoginResult result)
	    {
		    UIManager.instance.loading = false;
	        if (result.NewlyCreated)
            {
	            Debug.Log("New account");
	        }
            Debug.Log("Login success!");
            SetLoginMessage("Success!");
	    }

	    void OnLoginError(PlayFabError error)
	    {
		    UIManager.instance.loading = false;
	        switch (error.Error)
	        {
	            case PlayFabErrorCode.AccountNotFound:
	                SetLoginMessage("Username or password are incorrect");
	                break;
	            case PlayFabErrorCode.InvalidParams:
	                SetLoginMessage("Username or password are incorrect");
	                break;
	            case PlayFabErrorCode.AccountBanned:
	                SetLoginMessage("Account has been banned");
	                break;
	            default:
	                SetLoginMessage("Internal Error: Please try again");
	                break;
	        }
	        Debug.Log("[Playfab OnLoginError] Error: " + error.ErrorMessage + ", " + error.Error);
        }

		public void Register()
		{
            // Test password
		    UpdatePasswordCheck();
            accountView.passwordInfoBox.SetActive(false);

            if (!passwordValid)
            {
                SetRegisterMessage("Password is not valid");
                accountView.passwordInfoBox.SetActive(true);
                return;
            }
		    UIManager.instance.loading = true;

            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
		    request.Username = username;
		    request.Password = password;
		    request.Email = email;
		    request.TitleId = SceneManager.instance.playfabTitleId;
		    PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterResult, OnRegisterError);

            SetRegisterMessage("Registering");
            Debug.Log("Registering user: " + username + ", " + email + ", " + password);
		}

	    void OnRegisterResult(RegisterPlayFabUserResult result)
	    {
	        UIManager.instance.loading = false;
	        Debug.Log("New account");
	        SetRegisterMessage("Successfully registered!");
	    }

	    void OnRegisterError(PlayFabError error)
	    {
	        UIManager.instance.loading = false;
	        switch (error.Error)
	        {
                case PlayFabErrorCode.AccountNotFound:
	                SetRegisterMessage("Username or password are incorrect");
                    break;
                case PlayFabErrorCode.InvalidParams:
                    SetRegisterMessage("Username or password are incorrect");
                    break;
                case PlayFabErrorCode.AccountBanned:
                    SetRegisterMessage("Account has been banned");
                    break;
                default:
                    SetRegisterMessage("Internal Error: Please try again");
                    break;
            }
	        Debug.Log("[Playfab OnRegisterError] Error: " + error.ErrorMessage + ", " + error.Error);
	    }

    }
}
