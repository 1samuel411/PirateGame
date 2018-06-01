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

        public WeaponData curWeapon;

        void Start()
        {

        }

        void Update()
        {
            if (PlayerManager.instance.playerWeaponManager == null)
                return;

            if(InputManager.instance.player.GetButtonDown("Shoot"))
            {
                Attack();
            }

            curWeapon = PlayerManager.instance.playerWeaponManager.curWeapon;
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
                if (curWeapon.ammo <= 0)
                    return;
            }

            curWeapon.ammo--;

            lastAttackTime = Time.time + curWeapon.hitRate;

            if(attackAction != null)
                attackAction.Invoke();
        }
    }
}