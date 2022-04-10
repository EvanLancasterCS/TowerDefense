using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ARGS
// [0] projectiles
// [1] damage
// [2] spread
// [3] burstTime
// [4] range
// [5] fireRate
// [6] projectileeffecthelpers
public class ProjectileTower : BaseTower
{
    public static int NUM_ARGS = 7;
    private int baseProjectiles = 1; // number of projectiles shot per shot
    private float baseDamage = 1; // damage per projectile
    private float baseSpread = 0; // spread of the projectiles in degrees
    private float baseBurstTime = 0; // amount of time between each shot
    private float baseRange = 0; // #units to search for target
    private float baseFireRate = 0; // #shots to fire per second
    private string targetTag = "Enemy";

    private List<ProjectileEffect> projectileEffects = new List<ProjectileEffect>();

    private void Awake()
    {
        type = TowerType.Projectile;
    }

    public override void SetTower(Coordinate pos, int towerID, int qual, int upgradeID, bool retrievable, string towerName, object[] args)
    {
        base.SetTower(pos, towerID, qual, upgradeID, retrievable, towerName, args);

        if(args.Length != NUM_ARGS)
        {
            print("Error: #args invalid for this projectile tower");
            return;
        }
        if(!hasBeenSet)
        {
            hasBeenSet = true;
            baseProjectiles = (int)args[0];
            baseDamage = (int)args[1];
            baseSpread = (float)args[2];
            baseBurstTime = (float)args[3];
            baseRange = (float)args[4];
            baseFireRate = (float)args[5];
            tickRate = (int)(20f / baseFireRate);

            if (args[6] != null)
            {
                ProjectileEffect.Helper[] effectList = (ProjectileEffect.Helper[])args[6];
                for (int i = 0; i < effectList.Length; i++)
                {
                    ProjectileEffect.EffectType effectType = effectList[i].type;
                    object[] effectArgs = effectList[i].args;
                    ProjectileEffect effect = null;
                    if (effectType == ProjectileEffect.EffectType.explosive)
                        effect = new ExplosiveProjectileEffect();
                    else if (effectType == ProjectileEffect.EffectType.homing)
                        effect = new HomingProjectileEffect();
                    else if (effectType == ProjectileEffect.EffectType.piercing)
                        effect = new PiercingProjectileEffect();
                    else
                        Debug.Log("Error; projectile effect isn't assigned.");

                    effect.SetupEffect(effectArgs);
                    projectileEffects.Add(effect);
                }
            }
        }
        else
        {
            print("Error: tower has already been setup");
        }
    }


    // returns true if tick resolved in an action
    public override bool Tick()
    {
        if(base.Tick())
        {
            Collider[] overlaps = Physics.OverlapSphere(transform.position, GetRange());
            Transform closest = null;
            float distClosest = -1;
            for (int i = 0; i < overlaps.Length; i++)
            {
                if(overlaps[i].tag == targetTag)
                {
                    Enemy enemy = overlaps[i].GetComponent<Enemy>();
                    if(closest == null)
                    {
                        closest = overlaps[i].transform;
                        distClosest = enemy.DistanceFromGoal();//Vector3.Distance(closest.position, transform.position);
                        continue;
                    }
                    float distCurr = enemy.DistanceFromGoal();
                    if (distCurr < distClosest)
                    {
                        closest = overlaps[i].transform;
                        distClosest = distCurr;
                    }
                }
            }

            if (closest != null)
            {
                int numProj = GetNumProjectiles();
                for(int i = 0; i < numProj; i++)
                    StartCoroutine(CreateBulletAfterTime(i * GetBurstTime(), closest));
            }
            return true;
        }
        return false;
    }

    IEnumerator CreateBulletAfterTime(float time, Transform target)
    {
        yield return new WaitForSeconds(time);
        if (target != null)
        {
            GameObject projObj = Instantiate(TowerManager.instance.projectilePrefab);
            projObj.transform.position = transform.position + new Vector3(0, 1, 0);
            Projectile proj = projObj.GetComponent<Projectile>();

            proj.SetProjectile(target, "Enemy", 5f, 30f, GetDamage(), GetSpread());
            foreach (ProjectileEffect e in projectileEffects)
                proj.AttachEffect(e);

            soundFX.inst.playSound(id);
        }
        
    }

    public float GetSpread()
    {
        return baseSpread;
    }

    public float GetBurstTime()
    {
        return baseBurstTime;
    }

    public int GetNumProjectiles()
    {
        return baseProjectiles;
    }

    public float GetDamage()
    {
        return baseDamage;
    }

    public override float GetRange()
    {
        return baseRange;
    }

    public float GetFireRate()
    {
        return baseFireRate;
    }

    public float GetShotsPerSec()
    {
        return GetFireRate() * GetNumProjectiles();
    }

    public float GetDPS()
    {
        return (GetDamage() * GetNumProjectiles() * GetFireRate());
    }

    public override string GetStats()
    {
        string stats = "";

        float hexagonRange = (GetRange() / 2f) - 0.5f;

        stats += "<color=red><size=30>" + GetDPS() + "</size></color> DPS\n";

        float fireRate = GetShotsPerSec();
        if (fireRate == 1)
            stats += "<color=yellow>" + fireRate + "</color> shot per second\n";
        else if (fireRate > 1)
            stats += "<color=yellow>" + fireRate + "</color> shots per second\n";
        else if (fireRate < 1 && fireRate != 0)
            stats += "1 shot every <color=yellow>" + System.Decimal.Round((decimal)(1 / fireRate), 2) + "</color> seconds\n";

        stats += "Deals <color=yellow>" + GetDamage() + "</color> damage per shot\n";

        stats += "Range of <color=yellow>" + hexagonRange + "</color> hexagons.\n";

        stats += "\n";
        if (projectileEffects.Count == 0)
            stats += "<size=18>No Extra Effects</size>\n";
        else
        {
            for(int i = 0; i < projectileEffects.Count; i++)
            {
                stats += projectileEffects[i].GetStats() + "\n";
                /*if (i != projectileEffects.Count - 1)
                    stats += "\n";*/
            }
        }

        stats += "Tier <color=yellow>" + quality + "</color>\n";

        if (HasUpgrade())
            stats += "<size=16>Upgradable</size>";
        else
            stats += "<size=16>Not Upgradable</size>";

        return stats;
    }
}
