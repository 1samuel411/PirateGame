using UnityEngine;
using PirateGame.Managers;

namespace PirateGame.UI.Controllers
{
    public class InGameController : Controller
    {

        private void OnEnable()
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.activeScreens.Clear();
                UIManager.instance.AddScreenAdditive("InGameOverlay");
            }
        }

        private void OnDisable()
        {
            //if (UIManager.instance != null)
            //    UIManager.instance.UnloadScreenAdditive("InGameOverlay");
        }

    }
}