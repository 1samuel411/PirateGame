using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame
{
    /// <summary>
    /// Moves around a point marked as the target with an offset applied
    /// Takes input from the mouse and mouse scroll wheel to zoom in and out (offset)
    /// X and Y are reversed in the rotation field so keep that in mind
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {

        public Transform target;

        public Vector3 offsetMovement;

        public Vector2 clampYAmount = new Vector2(0, 360);
        public Vector2 clampXAmount = new Vector2(-70, 70);
        public float mouseMoveSpeed = 5f;
        private Vector2 mouseMovement;

        public Vector2 clampOffsetAmount = new Vector2(5, 20);
        public float offset = 15;
        public float offsetMoveAmount = 2.5f;
        public float offsetMoveSpeed = 5f;
        private float offsetTarget;

        void Start()
        {
            offsetTarget = offset;
        }

        void Update()
        {

            GetMouseInput();

            GetScrollInput();

            SetPosition();

        }

        void GetMouseInput()
        {
            mouseMovement = InputManager.instance.player.GetAxis2D("Mouse Y", "Mouse X");

            Vector3 angle = transform.eulerAngles;
            angle += (new Vector3(mouseMovement.x, mouseMovement.y) * mouseMoveSpeed * Time.deltaTime);

            angle.x = WrapAngle(angle.x);
            angle.x = ClampAngle(angle.x, clampXAmount.x, clampXAmount.y);
            angle.y = ClampAngle(angle.y, clampYAmount.x, clampYAmount.y);

            transform.eulerAngles = (angle);
        }

        void GetScrollInput()
        {
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
            
            transform.position = (target.position + offsetMovement) - (transform.forward * offset);
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

        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
    }
}
