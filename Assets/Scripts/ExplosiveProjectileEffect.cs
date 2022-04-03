using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectileEffect : ProjectileEffect
{
    public ExplosiveProjectileEffect()
    {
        type = EffectType.explosive;
    }

    public override void Activate(Vector3 pos)
    {

    }
}
