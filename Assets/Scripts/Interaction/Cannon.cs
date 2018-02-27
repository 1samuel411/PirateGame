using UnityEngine;

namespace PirateGame.Interactables
{
    public class Cannon : Interactable
    {
        [Header("Cannon")]
        public Vector2 clampXRotation;
        private Vector2 originalClampXRotation;
        public Vector2 clampYRotation;
        private Vector2 originalClampYRotation;

        public Transform barrelTransform;

        public override void InteractionTrigger()
        {
            if (activated)
            {
                // Force aim
                humanoid.forceAiming = true;

                // Clamp Rotation
                originalClampXRotation = new Vector2(transform.eulerAngles.y + clampXRotation.x, transform.eulerAngles.y + clampXRotation.y);
                humanoid.clampXRotation = clampXRotation;

                originalClampYRotation = humanoid.clampYRotation;
                humanoid.clampYRotation = clampYRotation;
            }
            else
            {
                // Disable Froce aim
                humanoid.forceAiming = false;

                // Restore Clamps
                humanoid.clampXRotation = originalClampXRotation;
                humanoid.clampYRotation = originalClampYRotation;
            }
        }

        void Update()
        {
            if(activated)
            {
                // Move barrel forward
                barrelTransform.transform.rotation = humanoid.forwardTransform.rotation;
            }
        }
    }
}