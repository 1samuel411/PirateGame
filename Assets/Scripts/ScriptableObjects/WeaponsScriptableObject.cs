﻿using System.Collections;
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

        public AmmoData[] ammoType;
    }

    [System.Serializable]
    public class AmmoData
    {
        [Space(20)]
        public Type ammoType;
        [HideInInspector]
        public int bullets = -1;
        public int defaultCount;
        public GameObject ammoPrefab;

        public enum Type
        {
            cannonBalls,
            miniCannonBalls,
            grapeShots
        }
    }

    [System.Serializable]
    public class WeaponData
    {
        [Space(30)]
        public bool defaultWeapon;
        public string name;
        public Sprite icon;
        public GameObject weaponPrefab;

        public Vector3 offsetHold;

        public float hitRate;

        public float spineOffset;

        public string[] attackAnimations;
        [ShowIf("canBlock", true)]
        public string[] blockAnimations;

        public bool gun;

        public bool canBlock;
        [ShowIf("canBlock", true)]
        public float blockTime;

        [ShowIf("gun", false)]
        public WeaponType weaponType;

        [ShowIf("gun", true)]
        public int[] muzzleFlashes;
        [ShowIf("gun", true)]
        public GunType gunType;
        [ShowIf("gun", true)]
        public ReloadType reloadType;
        [ShowIf("gun", true)]
        public AmmoData.Type ammoType;
        [ShowIf("gun", false)]
        [ShowIf("reloadType", ReloadType.individual)]
        public string reloadBeginAnimation;
        [ShowIf("gun", true)]
        public string reloadAnimation;
        [ShowIf("gun", false)]
        [ShowIf("reloadType", ReloadType.individual)]
        public string reloadEndAnimation;
        [ShowIf("gun", true)]
        public int ammoHeld;
        [ShowIf("gun", true)]
        public float reloadTime;

        [HideInInspector]
        public int ammo;
        [HideInInspector]
        public Transform muzzleTransform;

        [HideInInspector]
        public string skinName;

        public enum WeaponType
        {
            oneHanded
        }

        public enum GunType
        {
            pistol,
            shotgun
        }

        public enum ReloadType
        {
            whole,
            individual
        }
    }
}