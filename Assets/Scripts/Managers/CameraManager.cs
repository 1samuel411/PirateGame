﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class CameraManager : MonoBehaviour
    {

        public static CameraManager instance;

        public Transform cameraObject;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {

        }
    }
}
