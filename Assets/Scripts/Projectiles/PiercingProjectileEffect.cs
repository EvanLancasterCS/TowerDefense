using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// args[0] = number of targets to pierce
public class PiercingProjectileEffect : ProjectileEffect
{
    private int numPierce;

    public override void SetupEffect(object[] args)
    {
        type = EffectType.explosive;
        int _numPierce = (int)args[0];

        numPierce = _numPierce;
    }

    // should return true if projectile is allowed to die
    public override bool ActivateOnHit(Transform arrow, Transform hit)
    {
        Projectile myArrow = arrow.GetComponent<Projectile>();
        int piercesLeft = 0;
        if (!myArrow.TrackerExists(this))
        {
            myArrow.UpdateTracker(this, numPierce);
        }
        else
        {
            piercesLeft = (int)myArrow.GetTracker(this);
            myArrow.UpdateTracker(this, piercesLeft - 1);
        }
        piercesLeft = (int)myArrow.GetTracker(this);
        return piercesLeft <= 0;
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

        return "+" + (numPierce) + " <color=yellow>Piercing</color>";
    }
}
