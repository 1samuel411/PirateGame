using UnityEngine;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
    public class MainMenuController : Controller
    {

        public string playMenuController;
        public string optionsMenuController;

        public void Play()
        {
            UIManager.instance.ScreenSwitch(playMenuController);
        }

        public void Options()
        {
            UIManager.instance.ScreenSwitch(optionsMenuController);
        }

        public void Quit()
        {
            
        }
    }
}