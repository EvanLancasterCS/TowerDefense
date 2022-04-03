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
public class ProjectileTower : BaseTower
{
    public static int NUM_ARGS = 6;
    private int baseProjectiles = 1; // number of projectiles shot per shot
    private float baseDamage = 1; // damage per projectile
    private float baseSpread = 0; // spread of the projectiles in degrees
    private float baseBurstTime = 0; // amount of time between each shot
    private float baseRange = 0; // #units to search for target
    private float baseFireRate = 0; // #shots to fire per second
    private string targetTag = "Enemy";

    private void Awake()
    {
        type = TowerType.Projectile;
    }

    public override void SetTower(Coordinate pos, int towerID, int qual, string towerName, object[] args)
    {
        base.SetTower(pos, towerID, qual, towerName, args);

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
                    if(closest == null)
                    {
                        closest = overlaps[i].transform;
                        distClosest = Vector3.Distance(closest.position, transform.position);
                        continue;
                    }
                    float distCurr = Vector3.Distance(overlaps[i].transform.position, transform.position);
                    if(distCurr < distClosest)
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
                    StartCoroutine(CreateBulletAfterTime(i* GetBurstTime(), closest.position));
            }
            return true;
        }
        return false;
    }

    IEnumerator CreateBulletAfterTime(float time, Vector3 target)
    {
        yield return new WaitForSeconds(time);

        GameObject projObj = Instantiate(TowerManager.instance.projectilePrefab);
        projObj.transform.position = transform.position + new Vector3(0, 1, 0);
        Projectile proj = projObj.GetComponent<Projectile>();

        proj.SetProjectile(target, "Enemy", 5f, 15f, GetDamage(), baseSpread);
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

    public float GetRange()
    {
        return baseRange;
    }

    public float GetFireRate()
    {
        return baseFireRate;
    }

    public override string GetStats()
    {
        string stats = "";
        stats += "<i><size=10><color=yellow>Stats:</color></size></i>\n";
        stats += "Fire Rate: " + GetFireRate() + "\n";
        stats += "Damage: " + GetDamage() + "\n";
        stats += "Spread: " + GetSpread() + "\n";
        stats += "<i><size=10><color=yellow>Effects:</color></size></i>\n";
        stats += "None\n"; // work in prog; eventually just return the ToString of the projectile effects
        return stats;
    }
}
