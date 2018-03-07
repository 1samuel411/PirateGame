using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class CameraManager : MonoBehaviour
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
            if (cameraObject)
                cameraObject.target = PlayerManager.instance.playerEntity.transform;
        }
    }
}
