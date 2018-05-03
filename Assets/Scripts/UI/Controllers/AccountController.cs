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
	    public bool forgotPassword;

	    public bool rememberMe;

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
		    SetForgotPasswordMessage("Please enter your email to reset your password");

		    if (PlayerPrefs.HasKey("Username"))
		    {
		        rememberMe = true;
		        username = PlayerPrefs.GetString("Username");
		        password = PlayerPrefs.GetString("Password");
		        accountView.rememberMeToggle.isOn = rememberMe;
		        accountView.usernameFieldLogin.text = username;
		        accountView.passwordFieldLogin.text = password;
		    }
        }

        void Update()
		{
			accountView.loginViewObject.SetActive(login && !forgotPassword);
			accountView.registerViewObject.SetActive(!login && !forgotPassword);
            accountView.forgotPasswordViewObject.SetActive(forgotPassword);

		    if (Time.time >= passwordInfoBoxTime)
		    {
		        accountView.passwordInfoBox.SetActive(false);
		    }
		}

	    public void SetForgotPasswordMessage(string message)
	    {
	        accountView.forgotPasswordViewText.text = message;
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

	    public void SetForgotPassword(bool value)
	    {
	       forgotPassword = value;
	    }

	    public void UpdateRememberMe(bool value)
	    {
	        rememberMe = value;
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

	    public void ForgotPassword()
	    {
	        if (email == "")
	            return;

	        UIManager.instance.loading = true;

            SetForgotPasswordMessage("Sending you an email with instructions");
            SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest();
	        request.TitleId = SceneManager.instance.playfabTitleId;
	        request.Email = email;
            PlayFabClientAPI.SendAccountRecoveryEmail(request, OnForgotPasswordResult, OnForgotPasswordError);

            Debug.Log("Forgotten Password for email: " + email);
	    }

	    void OnForgotPasswordResult(SendAccountRecoveryEmailResult result)
	    {
	        UIManager.instance.loading = false;

	        accountView.emailFieldForgotPassword.text = "";
	        email = "";

	        forgotPassword = false;

            SetForgotPasswordMessage("Sent email!");
        }

        void OnForgotPasswordError(PlayFabError error)
	    {
	        UIManager.instance.loading = false;

            SetForgotPasswordMessage("Check that your email address is valid");

	        Debug.Log("Error found: " + error.ErrorMessage);
	    }

		public void Login()
		{
		    UIManager.instance.loading = true;

            SetLoginMessage("Logging in");
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
		    request.Username = username;
		    request.Password = password;
            request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
		    request.InfoRequestParameters.GetPlayerProfile = true;
            request.TitleId = SceneManager.instance.playfabTitleId;
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginResult, OnLoginError);

            Debug.Log("Logging user in: " + username + ", " + "*******");
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

	        PlayerManager.instance.PlayerLogin(result);

	        // Save Login Data
	        if (rememberMe)
	        {
	            PlayerPrefs.SetString("Username", username);
	            PlayerPrefs.SetString("Password", password);
	        }
	        else
	        {
	            // Delete
                PlayerPrefs.DeleteKey("Username");
                PlayerPrefs.DeleteKey("Password");
	        }
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
	            case PlayFabErrorCode.ServiceUnavailable:
	                SetLoginMessage("Could not connect");
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
		    request.DisplayName = username;
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

	        UIManager.instance.loading = true;

	        SetRegisterMessage("Finalizing");
            AddOrUpdateContactEmailRequest request = new AddOrUpdateContactEmailRequest();
	        request.EmailAddress = email;
            PlayFabClientAPI.AddOrUpdateContactEmail(request, AddContactEmailResult, AddContactEmailError);
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
	            case PlayFabErrorCode.EmailAddressNotAvailable:
	                SetRegisterMessage("Email Address not available");
	                break;
	            case PlayFabErrorCode.UsernameNotAvailable:
	                SetRegisterMessage("Username not available");
	                break;
	            default:
	                SetRegisterMessage("Internal Error: Please try again");
	                break;
	        }
	        Debug.Log("[Playfab OnRegisterError] Error: " + error.ErrorMessage + ", " + error.Error);
	    }

        void AddContactEmailResult(AddOrUpdateContactEmailResult result)
	    {
            Debug.Log("Completed registration");

	        UIManager.instance.loading = false;

	        SetRegisterMessage("Logging in!");
            Login();
        }

        void AddContactEmailError(PlayFabError error)
	    {
	        UIManager.instance.loading = false;

            Debug.Log("[Playfab OnAddOrUpdateContactEmail] Error: " + error.ErrorMessage);
	    }

    }
}
