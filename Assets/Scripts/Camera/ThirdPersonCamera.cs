using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using PirateGame.Managers;

namespace PirateGame
{
    /// <summary>
    /// Moves around a point marked as the target with an offset applied
    /// Takes input from the mouse and mouse scroll wheel to zoom in and out (offset)
    /// X and Y are reversed in the rotation field so keep that in mind
    /// </summary>
    public class ThirdPersonCamera : Base
    {

        public Transform target;

        public Vector3 offsetMovement;

        public Vector2 clampYAmount = new Vector2(0, 360);
        public Vector2 clampXAmount = new Vector2(-70, 70);
        private Vector2 curClampXAmount;
        public float mouseMoveSpeed = 5f;
        private Vector2 mouseMovement;

        public Vector2 clampOffsetAmount = new Vector2(5, 20);
        public float offset = 15;
        public float offsetMoveAmount = 2.5f;
        public float offsetMoveSpeed = 5f;
        private float offsetTarget;

        public float aimingOffsetAmount = 5;
        private float preAimingOffsetAmount = 5;
        public Vector3 aimingOffset;
        private Vector3 preAimingOffset;
        public bool canAim = true;
        public bool forceAim;
        public bool aiming;

        void Start()
        {
            offsetTarget = offset;
            curClampXAmount = clampXAmount;
        }

        void Update()
        {
            GetMouseInput();

            GetAim();

            GetScrollInput();

            SetPosition();
        }

        private bool lastAiming = false;
        void GetAim()
        {
            aiming = (forceAim || InputManager.instance.player.GetButton("Aim") && canAim);

            if (aiming != lastAiming)
            {
                lastAiming = aiming;
                if (aiming)
                {
                    preAimingOffset = offsetMovement;
                    preAimingOffsetAmount = offsetTarget;
                    offsetTarget = aimingOffsetAmount;
                    DOTween.To(() => offsetMovement, x => offsetMovement = x, aimingOffset, 01f);
                }
                else
                {
                    offsetTarget = preAimingOffsetAmount;
                    DOTween.To(() => offsetMovement, x => offsetMovement = x, preAimingOffset, 01f);
                }
            }
        }

        void GetMouseInput()
        {
            mouseMovement = InputManager.instance.player.GetAxis2D("Mouse Y", "Mouse X");

            Vector3 angle = transform.localRotation.eulerAngles;
            angle += (new Vector3(mouseMovement.x, mouseMovement.y) * mouseMoveSpeed);

            angle.x = WrapAngle(angle.x);
            angle.x = ClampAngle(angle.x, curClampXAmount.x, curClampXAmount.y);
            angle.y = WrapAngle(angle.y);
            angle.y = ClampAngle(angle.y, clampYAmount.x, clampYAmount.y);
            transform.localRotation = Quaternion.Euler(angle);
            PlayerManager.instance.playerEntity.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        void GetScrollInput()
        {
            if (aiming)
                return;

            if (InputManager.instance.player.GetButton("Zoom In"))
            {
                offsetTarget += offsetMoveAmount;
            }

            if (InputManager.instance.player.GetButton("Zoom Out"))
            {
                offsetTarget -= offsetMoveAmount;
            }

            offsetTarget = Mathf.Clamp(offsetTarget, clampOffsetAmount.x, clampOffsetAmount.y);
        }

        void SetPosition()
        {
            offset = Mathf.Lerp(offset, offsetTarget, offsetMoveSpeed * Time.deltaTime);
            Vector3 offsetMovementNew = target.position;
            offsetMovementNew += target.right * offsetMovement.x;
            offsetMovementNew += target.up * offsetMovement.y;
            offsetMovementNew += target.forward * offsetMovement.z;
            transform.position = (offsetMovementNew) - (transform.forward * offset);
        }

        public float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        private float UnwrapAngle(float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }

        public static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
    }
}
