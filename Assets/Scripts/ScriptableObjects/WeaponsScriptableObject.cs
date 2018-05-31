using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PirateGame.Interactables;

namespace PirateGame.ScriptableObjects
{
    public class WeaponsScriptableObject : ScriptableObject
    {
        public static string location = "Weapons";
        public static string resourcesLocation = "Assets/Resources/Weapons.asset";

        public WeaponData[] weaponData;
    }
}