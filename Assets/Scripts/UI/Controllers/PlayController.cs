using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;

namespace PirateGame.UI.Controllers
{
    public class PlayController : Controller
    {

        public string mainMenuController = "Menu";
        public string ipAddress;
        public string portAddress;

        public PlayMenuView playMenuView;

        public void JoinServer()
        {
            
        }

        public void HostServer()
        {
            
        }

        public void UpdateIp(string newIp)
        {
            ipAddress = newIp;
        }

        public void UpdatePort(string newPort)
        {
            portAddress = newPort;
        }

        public void Back()
        {
            UIManager.instance.ScreenSwitch(mainMenuController);
        }
    }
}