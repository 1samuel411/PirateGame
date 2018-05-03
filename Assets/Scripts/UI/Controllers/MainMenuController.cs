using UnityEngine;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
    public class MainMenuController : Controller
    {

        public string playMenuController;

        public void Play()
        {
            UIManager.instance.ScreenSwitch(playMenuController);
        }
        
    }
}