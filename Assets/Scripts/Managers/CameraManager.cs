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
            instance = this;
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (cameraObject && PlayerManager.instance.playerEntity)
                cameraObject.target = PlayerManager.instance.playerEntity.transform;
        }
    }
}
