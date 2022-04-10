using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// parent class for tiles to be able to easily interact with
public abstract class BaseTower : MonoBehaviour
{
    protected int id = -1;
    protected string tName;
    protected Coordinate tPos;
    protected int currHealth = 20;
    protected int maxHealth = 20;
    protected int quality = 1;
    protected int upgradeID = -1;
    protected bool retrievable = true;

    protected int tickRate = 1;
    private int currentTick = 0;

    public TowerType type;
    protected bool hasBeenSet = false;

    public enum TowerType { Home, Projectile, Wall };

    public virtual bool Tick()
    {
        currentTick += 1;
        if(currentTick >= tickRate)
        {
            currentTick = 0;
            return true;
        }
        return false;
    }

    public virtual void SetTower(Coordinate pos, int towerID, int qual, int _upgradeID, bool retrieve, string towerName, object[] args)
    {
        id = towerID;
        tName = towerName;
        tPos = pos;
        quality = qual;
        retrievable = retrieve;
        upgradeID = _upgradeID;
    }

    public void SetPos(Coordinate pos)
    {
        tPos = pos;
    }

    // returns true if dead
    public virtual bool TakeDamage(int dmg)
    {
        currHealth -= dmg;
        return IsDead();
    }

    // returns true if dead
    public bool IsDead()
    {
        return currHealth <= 0;
    }

    public string GetSlug()
    {
        string slug = tName.Replace(" ", "_");
        return slug.ToLower();
    }

    public virtual string GetName() => tName;
    public virtual int GetQuality() => quality;
    public virtual bool IsRetrievable() => retrievable;
    public virtual float GetRange() => 0;

    public bool HasUpgrade() => upgradeID != -1;
    public int GetID() => id;
    public int GetUpgradeID() => upgradeID;

    // per-tower stats change, so require implementation for description
    public abstract string GetStats();
}
