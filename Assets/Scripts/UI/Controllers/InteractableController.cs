using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class InteractableController : Controller
    {

        public UI.Views.InteractableView interactableView;

        public bool interactable;

        void Update()
        {
            CheckInteractable();

            UpdateUI();
        }

        void CheckInteractable()
        {
            interactable = PirateGame.Managers.PlayerManager.instance.playerEntity.interactionColliders.Length > 0;
        }

        void UpdateUI()
        {
            interactableView.interactableText.gameObject.SetActive(interactable);
        }

    }
}