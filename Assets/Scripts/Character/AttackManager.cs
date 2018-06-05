using PirateGame;
using PirateGame.Interactables;
using PirateGame.Managers;
using PirateGame.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Character
{
    public class AttackManager : Base
    {

        public Action attackAction;
        public Action blockAction;

        public WeaponData curWeapon;

        void Start()
        {

        }

        void Update()
        {
            if (PlayerManager.instance.playerWeaponManager == null)
                return;

            curWeapon = PlayerManager.instance.playerWeaponManager.curWeapon;

            if (InputManager.instance.player.GetButton("Shoot"))
            {
                Attack();
            }

            if (InputManager.instance.player.GetButton("Block"))
            {
                Block();
            }
        }

        private float lastAttackTime;
        public void Attack()
        {
            if (Time.time < lastAttackTime)
                return;
            if (curWeapon.name == "")
                return;
            if (curWeapon.gun)
            {
                if (curWeapon.ammo <= 0)
                {
                    Reload();
                    return;
                }
                curWeapon.ammo--;

                // Spawn muzzle
                if (curWeapon.muzzleFlashes.Length > 0 && curWeapon.muzzleTransform != null)
                {
                    SpawnableObjectInfo muzzleFlash = ObjectManager.instance.Instantiate("MuzzleFlash" + UnityEngine.Random.Range(1, curWeapon.muzzleFlashes.Length), curWeapon.muzzleTransform.position);
                    muzzleFlash.transform.SetParent(curWeapon.muzzleTransform);
                    muzzleFlash.transform.rotation = curWeapon.muzzleTransform.rotation;
                    muzzleFlash.transform.localEulerAngles += new Vector3(0, 180, 0);
                    muzzleFlash.transform.SetParent(null);
                }
            }

            lastAttackTime = Time.time + curWeapon.hitRate;

            if(attackAction != null)
                attackAction.Invoke();
        }

        private float lastBlockTime;
        public void Block()
        {
            if (Time.time < lastAttackTime)
                return;
            if (Time.time < lastBlockTime)
                return;
            if (curWeapon.name == "")
                return;
            if (!curWeapon.canBlock)
                return;

            lastBlockTime = Time.time + curWeapon.blockTime;

            if (blockAction != null)
                blockAction.Invoke();
        }

        public void Reload()
        {

        }
    }
}