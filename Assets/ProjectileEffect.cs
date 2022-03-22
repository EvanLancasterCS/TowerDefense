using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileEffect
{
    public EffectType type;
    public enum EffectType { explosive };

    public abstract void Activate(Vector3 pos);
}
