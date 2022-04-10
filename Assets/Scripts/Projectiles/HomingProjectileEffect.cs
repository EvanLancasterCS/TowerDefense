using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// args[0] = homing strength
public class HomingProjectileEffect : ProjectileEffect
{
    private int homingStrength;
    private const int acceptableRotationDiff = 5;
    public override void SetupEffect(object[] args)
    {
        type = EffectType.homing;
        int _homingStrength = (int)args[0];

        homingStrength = _homingStrength;
    }

    // should return true if projectile is allowed to die
    public override bool ActivateOnHit(Transform arrow, Transform hit)
    {
        return true;
    }

    public override void ActivateTraveling(Transform arrow, Transform target)
    {
        float currentRotation = arrow.eulerAngles.y;
        Vector3 vecDiff = target.position - arrow.position;
        float desiredRotation = Mathf.Atan2(vecDiff.x, vecDiff.z) * Mathf.Rad2Deg;
        if (Mathf.Abs(desiredRotation - currentRotation) > Mathf.Abs(desiredRotation - currentRotation + 360)) // just brings it closer because return value is between pi and -pi
            desiredRotation += 360;


        float angleDiff = desiredRotation - currentRotation;
        float dir = angleDiff < 0 ? -1 : 1;
        if(Mathf.Abs(angleDiff) > acceptableRotationDiff)
        {
            currentRotation += (homingStrength) * Time.deltaTime * dir * 300;
        }
        arrow.eulerAngles = new Vector3(0, currentRotation, 0);
    }

    public override void ActivateCreation(Transform arrow)
    {
        // nothing to do
    }

    public override string GetStats()
    {

        return "+" + homingStrength + " <color=yellow>Homing</color>";
    }
}
