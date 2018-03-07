using System.Collections;
using System.Collections.Generic;
using PirateGame.Effects;
using PirateGame.Managers;
using PirateGame.Projectiles;
using UnityEngine;

namespace PirateGame.Shootables
{
    public class Shootable : Base, IShootable
    {

        public float fireRate;

        public Transform muzzleTransform;
        public Effect muzzleEffect;
        public Projectile projectile;

        public Animator animator;
        public string animationName = "Fire";

        public bool canShoot = true;

        private float timeAtShoot;

        void Start()
        {

        }

        void Update()
        {
            if (Time.time >= (timeAtShoot + fireRate))
                canShoot = true;
        }

        public void Shoot()
        {
            if (!canShoot)
                return;

            canShoot = false;

            timeAtShoot = Time.time;

            if (animator)
                animator.CrossFadeInFixedTime(animationName, 0.0f);

            ObjectManager.instance.Instantiate(muzzleEffect.GetGameObject(), muzzleTransform.position,
                muzzleTransform.rotation);
            ObjectManager.instance.Instantiate(projectile.GetGameObject(), muzzleTransform.position,
                muzzleTransform.rotation);
        }

        public bool GetCanShoot()
        {
            return canShoot;
        }

        public float GetFireRate()
        {
            return fireRate;
        }

        public Transform GetMuzzle()
        {
            return muzzleTransform;
        }

        public IEffect GetMuzzleEffect()
        {
            return muzzleEffect;
        }

        public IProjectile GetProjectile()
        {
            return projectile;
        }
    }
}