using UnityEngine;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
    public class OptionsController : Controller
    {

        public string mainMenuController = "Menu";

        public void Back()
        {
            UIManager.instance.ScreenSwitch(mainMenuController);
        }
    }
}