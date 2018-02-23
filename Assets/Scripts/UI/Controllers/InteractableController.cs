using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class InteractableController : Controller
    {

        public UI.Views.InteractableView interactableView;

        public bool interactable;
        public bool interacting;
        public bool interactingFinal;
        public bool interactingStopping;

        void Update()
        {
            CheckInteractable();

            UpdateUI();
        }

        void CheckInteractable()
        {
            interactable = PirateGame.Managers.PlayerManager.instance.playerEntity.interactionColliders.Length > 0;
            interacting = PirateGame.Managers.PlayerManager.instance.playerEntity.interacting;
            interactingStopping = PirateGame.Managers.PlayerManager.instance.playerEntity.interactingStopping;
            interactingFinal = PirateGame.Managers.PlayerManager.instance.playerEntity.interactingFinal;
        }

        void UpdateUI()
        {
            interactableView.interactableText.gameObject.SetActive(interactable && !interacting);
            interactableView.stopInteractingText.gameObject.SetActive(interactingFinal && !interactingStopping);
        }

    }
}