using PirateGame.Interactables;
using PirateGame.Managers;
using PirateGame.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Entity
{
    public class PlayerWeaponManager : NetworkingBase
    {

        public WeaponsScriptableObject weaponsList;

        public WeaponData defaultWeapon;

        public WeaponData weaponOne;
        public WeaponData weaponTwo;
        public WeaponData weaponThree;

        public int cannonBalls;
        public int miniCannonBalls;
        public int grapeShots;

        public int _equipedWeapon = 0;
        public int equipedWeapon
        {
            get
            {
                return _equipedWeapon;
            }
            set
            {
                _equipedWeapon = value;
                _equipedWeapon = Mathf.Clamp(_equipedWeapon, 0, 3);
                WeaponChanged();
            }
        }

        public GameObject defaultWeaponGO;
        public GameObject weaponOneGO;
        public GameObject weaponTwoGO;
        public GameObject weaponThreeGO;

        public WeaponData curWeapon
        {
            get
            {
                if (equipedWeapon == 0)
                    return defaultWeapon;
                if (equipedWeapon == 1)
                    return weaponOne;
                if (equipedWeapon == 2)
                    return weaponTwo;
                if (equipedWeapon == 3)
                    return weaponThree;

                return null;
            }
        }

        private void Awake()
        {

        }

        private void Start()
        {
            
        }

        private bool initialized = false;
        private void Update()
        {
            if (initialized == false && PlayerManager.instance.playablePlayer != null && PlayerManager.instance.playablePlayer.rightHandWeaponHolder != null)
            {
                // Set default weapon
                defaultWeapon = GetDefaultWeapon();
                SetWeapon(0, defaultWeapon);
                equipedWeapon = 0;
                initialized = true;
            }

            if (InputManager.instance.player.GetButtonDown("DefaultWeapon"))
                equipedWeapon = 0;

            if (InputManager.instance.player.GetButtonDown("WeaponOne"))
                equipedWeapon = 1;

            if (InputManager.instance.player.GetButtonDown("WeaponTwo"))
                equipedWeapon = 2;

            if (InputManager.instance.player.GetButtonDown("WeaponThree"))
                equipedWeapon = 3;

            if (InputManager.instance.player.GetButtonDown("WeaponAdd"))
                equipedWeapon++;

            if (InputManager.instance.player.GetButtonDown("WeaponRemove"))
                equipedWeapon--;

            if(InputManager.instance.player.GetButtonDown("Interact"))
            {
                PickupWeapon();
            }

            if(curWeapon != null && PlayerManager.instance.playablePlayer.spineRotator != null)
            {
                if(curWeapon.name == "")
                    PlayerManager.instance.playablePlayer.spineRotator.SetOffset(0);
                else
                    PlayerManager.instance.playablePlayer.spineRotator.SetOffset(curWeapon.spineOffset);
            }
        }

        void PickupWeapon()
        {
            if(PlayerManager.instance.playerEntity.pickupableColliders.Count > 0)
            {
                WeaponPickupable weaponPickupable = PlayerManager.instance.playerEntity.pickupableColliders[0].GetComponent<WeaponPickupable>();
                if(weaponPickupable)
                    EquipWeapon(weaponPickupable.weaponName, weaponPickupable.gameObject, weaponPickupable.muzzle, weaponPickupable.ammo);
                else
                {
                    AmmoPickupable ammoPickupable = PlayerManager.instance.playerEntity.pickupableColliders[0].GetComponent<AmmoPickupable>();
                    if (ammoPickupable)
                        EquipAmmo(ammoPickupable.ammoType, ammoPickupable.gameObject, ammoPickupable.ammo);
                }
            }
        }

        public WeaponData GetWeaponData(string weaponName)
        {
            for(int i = 0; i < PlayerManager.instance.weaponsList.weaponData.Length; i++)
            {
                if(PlayerManager.instance.weaponsList.weaponData[i].name == weaponName)
                {
                    return PlayerManager.instance.weaponsList.weaponData[i];
                }
            }

            return null;
        }

        public AmmoData GetAmmoData(AmmoData.Type ammoType)
        {
            for(int i = 0; i < PlayerManager.instance.weaponsList.ammoType.Length; i++)
            {
                if(PlayerManager.instance.weaponsList.ammoType[i].ammoType == ammoType)
                {
                    return PlayerManager.instance.weaponsList.ammoType[i];
                }
            }

            return null;
        }

        public int GetCurrentAmmo()
        {
            if (curWeapon.ammoType == AmmoData.Type.cannonBalls)
                return cannonBalls;
            if (curWeapon.ammoType == AmmoData.Type.miniCannonBalls)
                return miniCannonBalls;
            if (curWeapon.ammoType == AmmoData.Type.grapeShots)
                return grapeShots;

            return 0;
        }

        public WeaponData GetDefaultWeapon()
        {
            for (int i = 0; i < PlayerManager.instance.weaponsList.weaponData.Length; i++)
            {
                if (PlayerManager.instance.weaponsList.weaponData[i].defaultWeapon)
                {
                    return PlayerManager.instance.weaponsList.weaponData[i];
                }
            }

            return null;
        }

        public void WeaponChanged()
        {
            // Do weapon equip
            WeaponData weaponData = GetWeapon(equipedWeapon);

            CameraManager.instance.cameraObject.canAim = !weaponData.canBlock;

            if(defaultWeaponGO)
                defaultWeaponGO.SetActive(false);
            if(weaponOneGO)
                weaponOneGO.SetActive(false);
            if(weaponTwoGO)
                weaponTwoGO.SetActive(false);
            if(weaponThreeGO)
                weaponThreeGO.SetActive(false);
            
            if (weaponData == null)
                return;

            if (equipedWeapon == 0 && defaultWeaponGO != null)
                defaultWeaponGO.SetActive(true);

            if (equipedWeapon == 1 && weaponOneGO != null)
                weaponOneGO.SetActive(true);

            if (equipedWeapon == 2 && weaponTwoGO != null)
                weaponTwoGO.SetActive(true);

            if (equipedWeapon == 3 && weaponThreeGO != null)
                weaponThreeGO.SetActive(true);
        }

        public void EquipWeapon(string weaponToEquip, GameObject go, Transform muzzle = null, int ammo = -1)
        {
            WeaponData weapon = GetWeaponData(weaponToEquip);

            weapon.muzzleTransform = muzzle;

            if (ammo == -1)
                weapon.ammo = weapon.ammoHeld;
            else
                weapon.ammo = ammo;

            if (weaponOne.name != "")
            {
                if (weaponTwo.name != "")
                {
                    if (weaponThree.name != "")
                    {
                        // All weapons equiped, replace current weapon
                        if (equipedWeapon == 0)
                            return;
                        SetWeapon(equipedWeapon, weapon);
                    }
                    else
                    {
                        SetWeapon(3, weapon);
                    }
                }
                else
                {
                    SetWeapon(2, weapon);
                }
            }
            else
            {
                SetWeapon(1, weapon);
            }

            Destroy(go);
        }

        public void EquipAmmo(AmmoData.Type ammoToEquip, GameObject go, int ammo = -1)
        {
            AmmoData ammunitionData = GetAmmoData(ammoToEquip);
            if (ammo == -1)
                ammo = ammunitionData.defaultCount;

            switch(ammoToEquip)
            {
                case (AmmoData.Type.cannonBalls):
                    cannonBalls += ammo;
                    break;
                case (AmmoData.Type.miniCannonBalls):
                    miniCannonBalls += ammo;
                    break;
                case (AmmoData.Type.grapeShots):
                    grapeShots += ammo;
                    break;
            }

            Destroy(go);
        }

        public WeaponData GetWeapon(int i)
        {
            if (i == 0)
                return defaultWeapon;
            if (i == 1)
                return weaponOne;
            if (i == 2)
                return weaponTwo;
            if (i == 3)
                return weaponThree;

            return null;
        }

        public void SetWeapon(int i, WeaponData weaponData)
        {
            GameObject weaponGO = GameObject.Instantiate(weaponData.weaponPrefab);
            weaponGO.transform.localScale = Vector3.one;
            weaponGO.transform.SetParent(PlayerManager.instance.playablePlayer.rightHandWeaponHolder);
            if(weaponData.muzzleTransform != null)
                weaponData.muzzleTransform = weaponGO.transform.FindChildByRecursive(weaponData.muzzleTransform.name);
            Destroy(weaponGO.GetComponent<Rigidbody>());
            Destroy(weaponGO.GetComponent<Collider>());
            weaponGO.transform.localPosition = weaponData.offsetHold;
            weaponGO.transform.localRotation = Quaternion.identity;
            
            if (i == 0)
            {
                defaultWeapon = weaponData;
                defaultWeaponGO = weaponGO;
            }
            if (i == 1)
            {
                weaponOne = weaponData;
                weaponOneGO = weaponGO;
            }
            if (i == 2)
            {
                weaponTwo = weaponData;
                weaponTwoGO = weaponGO;
            }
            if (i == 3)
            {
                weaponThree = weaponData;
                weaponThreeGO = weaponGO;
            }

            equipedWeapon = i;
        }

    }
} 