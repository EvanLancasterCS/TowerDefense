using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ARGS
// [0] health
// [1] invincible
public class WallTower : BaseTower
{
    public static int NUM_ARGS = 2;
    private bool invincible;

    public override void SetTower(Coordinate pos, int towerID, int qual, int upgradeID, bool retrievable, string towerName, object[] args)
    {
        base.SetTower(pos, towerID, qual, upgradeID, retrievable, towerName, args);

        if (args.Length != NUM_ARGS)
        {
            print("Error: #args invalid for this projectile tower");
            return;
        }
        if (!hasBeenSet)
        {
            hasBeenSet = true;
            
            currHealth = (int)args[0];
            maxHealth = (int)args[0];
            invincible = (bool)args[1];
        }
        else
        {
            print("Error: tower has already been setup");
        }
    }

    public override bool TakeDamage(int dmg)
    {
        if (invincible)
            return false;
        return base.TakeDamage(dmg);
    }

    public override string GetStats()
    {
        if (invincible)
            return "Invincible";
        return currHealth + " / " + maxHealth;
    }
}
