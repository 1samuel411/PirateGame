using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class CameraManager : Base
    {

        public static CameraManager instance;

        public ThirdPersonCamera cameraObject;

        void Awake()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (cameraObject && PlayerManager.instance.playerEntity)
            {
                cameraObject.gameObject.SetActive(true);
                cameraObject.target = PlayerManager.instance.playerEntity.transform;
            }
            else
            {
                cameraObject.gameObject.SetActive(false);
            }
        }
    }
}
