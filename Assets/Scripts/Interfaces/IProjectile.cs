using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
