using PirateGame.Shootables;
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

        public Shootable shootable;

        public override void InteractionTrigger()
        {
            if (activated)
            {
                // Force aim
                humanoid.forceAiming = true;

                // Clamp Rotation
                originalClampXRotation = humanoid.clampXRotation;
                float yRot = ThirdPersonCamera.WrapAngle(transform.eulerAngles.y);
                humanoid.clampXRotation = new Vector2(yRot + clampXRotation.x, yRot + clampXRotation.y); ;

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

        public override void SendInput(string inputAction)
        {
            if (inputAction == "Shoot")
            {
                Fire();
            }
        }

        public void Fire()
        {
            shootable.Shoot();
        }
    }
}