using System.Collections;
using System.Collections.Generic;
using PirateGame.Entity;
using UnityEngine;

public class Projectile : Entity, IProjectile
{

    [Header("Projectile")]
    public int damageAmount;

    public float autoDestructTime;

    public float speed;

    public Effect impactEffect;

    private float destructTimer;
    new void Update()
    {
        destructTimer += Time.deltaTime;
        if(destructTimer >= autoDestructTime)
            gameObject.SetActive(false);

        base.Update();

    }

    void OnEnable()
    {
        SetAutoDestructTime();
        StartCoroutine(ResetRigidbody());
    }

    IEnumerator ResetRigidbody()
    {
        yield return null;
        velocityVector = transform.forward * speed;
    }

    public override void OnCollisionEnter()
    {
        Debug.Log("[Projectile] Collision");
        ObjectManager.instance.Instantiate(impactEffect.GetGameObject(), transform.position);
        gameObject.SetActive(false);
    }

    /* ------------------------------------------------------ */
    /* ------------------------------------------------------ */

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    public float GetAutoDestructTime()
    {
        return autoDestructTime;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetGravity()
    {
        if (hasCustomGravity)
            return gravity;
        else
            return Mathf.Abs(Physics.gravity.y);
    }

    public void SetAutoDestructTime()
    {
        destructTimer = 0;
    }

    public IEffect GetImpactEffect()
    {
        return impactEffect;
    }
}
