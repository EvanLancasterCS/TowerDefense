using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// args[0] = damage
// args[1] = radius
public class ExplosiveProjectileEffect : ProjectileEffect
{
    private int damage;
    private float radius;

    public override void SetupEffect(object[] args)
    {
        type = EffectType.explosive;
        int _damage = (int)args[0];
        float _radius = (float)args[1];

        damage = _damage;
        radius = _radius;
    }

    // should return true if projectile is allowed to die
    public override bool ActivateOnHit(Transform arrow, Transform hit)
    {
        GameObject explosionPrefab = ResourceLoader.instance.GetExplosionPrefab();
        GameObject myExplosion = GameObject.Instantiate(explosionPrefab);
        myExplosion.transform.position = hit.position;
        ExplosionEffect exEffect = myExplosion.GetComponent<ExplosionEffect>();
        exEffect.Set(damage, radius, hit.tag);

        return true;
    }

    public override void ActivateTraveling(Transform arrow, Transform target)
    {
        // nothing to do
    }

    public override void ActivateCreation(Transform arrow)
    {
        // nothing to do
    }

    public override string GetStats()
    {

        return "+" + damage + " <color=yellow>Explosive</color>";
    }
}
