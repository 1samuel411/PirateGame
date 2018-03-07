
using PirateGame.Effects;
using PirateGame.Projectiles;
using UnityEngine;

namespace PirateGame.Shootables
{
    public interface IShootable
    {

        void Shoot();

        float GetFireRate();

        bool GetCanShoot();

        Transform GetMuzzle();

        IEffect GetMuzzleEffect();

        IProjectile GetProjectile();

    }
}