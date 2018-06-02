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

            if (InputManager.instance.player.GetButtonDown("Shoot"))
            {
                Attack();
            }

            if (InputManager.instance.player.GetButtonDown("Block"))
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
            if(curWeapon.gun)
            {
                //if (curWeapon.ammo <= 0)
                //    return;

                curWeapon.ammo--;
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
    }
}