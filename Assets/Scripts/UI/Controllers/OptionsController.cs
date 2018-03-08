using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;

namespace PirateGame.UI.Controllers
{
    public class OptionsController : Controller
    {

        public string mainMenuController = "Menu";

        public OptionsView optionsView;

        public override void Enabled()
        {
            optionsView.nameInputField.text = PlayerManager.instance.user.username;
        }

        public void UpdateUsername(string newName)
        {
            PlayerManager.instance.user.username = newName;
        }

        public void Back()
        {
            UIManager.instance.ScreenSwitch(mainMenuController);
        }
    }
}