using System.Collections;
using System.Collections.Generic;
using PirateGame.Effects;
using UnityEngine;

namespace PirateGame.Projectiles
{
    public interface IProjectile
    {

        GameObject GetGameObject();

        int GetDamageAmount();

        float GetAutoDestructTime();

        float GetSpeed();

        float GetGravity();

        void SetAutoDestructTime();

        IEffect GetImpactEffect();
    }
}