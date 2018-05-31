using PirateGame.Managers;
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
        public bool pickupable;

        void Update()
        {
            if(PlayerManager.instance.playerEntity)
                CheckInteractable();

            UpdateUI();
        }

        void CheckInteractable()
        {
            pickupable = PlayerManager.instance.playerEntity.pickupableColliders.Count > 0;

            interactable = PlayerManager.instance.playerEntity.interactionColliders.Count > 0;
            interacting = PlayerManager.instance.playerEntity.interacting;
            interactingStopping = PlayerManager.instance.playerEntity.interactingStopping;
            interactingFinal = PlayerManager.instance.playerEntity.interactingFinal;
        }

        void UpdateUI()
        {
            interactableView.interactableText.gameObject.SetActive(interactable && !interacting && !pickupable);
            interactableView.stopInteractingText.gameObject.SetActive(interactingFinal && !interactingStopping && !pickupable);

            interactableView.pickupText.gameObject.SetActive(pickupable);
        }

    }
}