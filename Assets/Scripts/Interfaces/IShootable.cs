
using UnityEngine;

public interface IShootable
{

    void Shoot();

    float GetFireRate();

    bool GetCanShoot();

    Transform GetMuzzle();

    IEffect GetMuzzleEffect();

    IProjectile GetProjectile();

}
