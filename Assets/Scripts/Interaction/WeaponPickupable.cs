using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PirateGame.Interactables
{
	public class WeaponPickupable : Base
    {

        public string weaponName;

        void Start()
        {

        }

        void Update()
        {

        }
    }

    [System.Serializable]
    public class WeaponData
    {
        public bool defaultWeapon;
        public string name;
        public string skinName;
        public Sprite icon;
        public GameObject weaponPrefab;

        public Vector3 offsetHold;

        public float hitRate;

        public bool gun;
        [ShowIf("gun", true)]
        public int ammoHeld;
        [ShowIf("gun", true)]
        public float reloadTime;
    }
}