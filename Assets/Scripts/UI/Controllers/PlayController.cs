using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;

namespace PirateGame.UI.Controllers
{
    public class PlayController : Controller
    {

        public string mainMenuController = "Menu";
        public string ipAddress;
        public int portAddress;

        public PlayMenuView playMenuView;

        public override void Enabled()
        {
            playMenuView.ipField.text = ipAddress;
            playMenuView.portField.text = portAddress.ToString();
        }

        public void JoinServer()
        {
            PNetworkManager.instance.networkAddress = ipAddress;
            PNetworkManager.instance.networkPort = portAddress;
            PNetworkManager.instance.PStartClient();
        }

        public void HostServer()
        {
            PNetworkManager.instance.networkAddress = ipAddress;
            PNetworkManager.instance.networkPort = portAddress;
            PNetworkManager.instance.PStartHost();
        }

        public void UpdateIp(string newIp)
        {
            ipAddress = newIp;
        }

        public void UpdatePort(string newPort)
        {
            portAddress = int.Parse(newPort);
        }

        public void Back()
        {
            UIManager.instance.ScreenSwitch(mainMenuController);
        }
    }
}