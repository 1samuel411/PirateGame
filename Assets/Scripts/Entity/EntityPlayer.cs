using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Entity
{
    /// <summary>
    /// Get input from the user and translate it to movement
    /// 
    /// Includes
    /// - Rewired input
    /// 
    /// </summary>
    public class EntityPlayer : EntityHumanoid
    {
        public Transform camera;

        public new void Awake()
        {
            base.Awake();
            
        }

        public new void Start()
        {
            base.Start();
        }

        public new void Update()
        {
            base.Update();

            CheckInput();
        }

        void CheckInput()
        {
            inputVelocity = InputManager.instance.player.GetAxis2D("Horizontal", "Vertical");

            if (inputVelocity.y != 0)
            {
                // Rotate towards camera's direction
                transform.eulerAngles = new Vector3(0, camera.eulerAngles.y, 0);
            }
        }
    }
}
