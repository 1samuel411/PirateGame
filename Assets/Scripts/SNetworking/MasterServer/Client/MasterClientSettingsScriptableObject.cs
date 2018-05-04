using System;
using System.Collections.Generic;
using UnityEngine;

namespace SNetwork.Client
{

    [Serializable]
    public class MasterClientSettingsScriptableObject : ScriptableObject
    {
        public static string location = "MasterClientSettings";
        public static string resourcesLocation = "Assets/Plugins/SNetwork/Resources/MasterClientSettings.asset";

        [Header("Settings")]
        public int maxConnAttempts = 5;
        public float retryTime = 0.5f;

        [Header("Master Server Info")]
        public string ipAddress = "0.0.0.0";
        public int port = 1000;
    }
}