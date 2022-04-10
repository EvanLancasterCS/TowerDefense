using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileEffect
{
    public EffectType type;
    public enum EffectType { explosive, piercing, homing };

    // args specified by each subclass
    public abstract void SetupEffect(object[] args);

    // should return true if projectile is allowed to die
    public abstract bool ActivateOnHit(Transform arrow, Transform hit);

    // activated every frame on traveling
    public abstract void ActivateTraveling(Transform arrow, Transform target);

    // activated once on creation
    public abstract void ActivateCreation(Transform arrow);

    public abstract string GetStats();

    public struct Helper
    {
        public EffectType type;
        public object[] args;

        public Helper(EffectType _type, object[] _args)
        {
            type = _type;
            args = _args;
        }
    }
}
