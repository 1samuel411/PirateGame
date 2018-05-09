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
        [Header("North America")]
        public string ipAddressNA = "0.0.0.0";
        public int portNA = 1000;

        [Header("South America")]
        public string ipAddressSA = "0.0.0.0";
        public int portSA = 1000;

        [Header("Asia")]
        public string ipAddressAS = "0.0.0.0";
        public int portAS = 1000;

        [Header("Europe")]
        public string ipAddressEU = "0.0.0.0";
        public int portEU = 1000;

        [Header("Australia")]
        public string ipAddressAU = "0.0.0.0";
        public int portAU = 1000;
    }
}