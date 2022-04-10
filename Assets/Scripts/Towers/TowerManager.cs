using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;
    public GameObject towerPrefab;

    public GameObject projectilePrefab;

    [SerializeField] private Transform savedTowers;

    public List<BaseTower> unplacedTowers = new List<BaseTower>();

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // combines towers if possible
    // if they are the same, destroys both, removes from inventory, creates new tower and returns ID in unplacedtowers
    public int CombineTowers(int id1, int id2)
    {
        BaseTower t1 = unplacedTowers[id1];
        BaseTower t2 = unplacedTowers[id2];
        if(t1.GetID() == t2.GetID() && t1.HasUpgrade())
        {
            UIManager.instance.DestroyCardIfExists(t1);
            UIManager.instance.DestroyCardIfExists(t2);

            // if we add tower specific modifiers to stats, we should try to retrieve the ones from previous towers here
            int upgradeID = t1.GetUpgradeID();

            Destroy(t1.gameObject);
            Destroy(t2.gameObject);

            int newTowerID = CreateTower(upgradeID);

            return newTowerID;
        }
        return -1;
    }

    // returns index in unplaced array
    public int CreateTower(int towerID)
    {
        string towerName = TowerPresets.GetTowerName(towerID);
        object[] towerArgs = TowerPresets.GetTowerArgs(towerID);
        int towerQual = TowerPresets.GetTowerQuality(towerID);
        int towerUpgradeID = TowerPresets.GetTowerUpgradeID(towerID);
        bool towerRetrievable = TowerPresets.GetTowerRetrievable(towerID);


        GameObject newTower = GameObject.Instantiate(TowerManager.instance.towerPrefab, savedTowers);
        newTower.transform.localPosition = Vector3.zero;
        BaseTower tower = null;
        BaseTower.TowerType type = TowerPresets.GetTowerType(towerID);

        // Add appropriate behaviour for tower
        if (type == BaseTower.TowerType.Projectile)
            tower = newTower.AddComponent<ProjectileTower>();
        else if (type == BaseTower.TowerType.Home)
            tower = newTower.AddComponent<HomeTower>();
        else if (type == BaseTower.TowerType.Wall)
            tower = newTower.AddComponent<WallTower>();
        else
            print("Error: Type isn't any tower type"); // shouldn't ever be here unless you do some weird casting

        tower.SetTower(new Coordinate(5000, 5000), towerID, towerQual, towerUpgradeID, towerRetrievable, towerName, towerArgs);

        // Request visual from resources and place in object
        GameObject visualPrefab = ResourceLoader.instance.GetTowerModel(towerID);
        GameObject.Instantiate(visualPrefab, newTower.transform);

        unplacedTowers.Add(tower);
        return unplacedTowers.Count - 1;
    }

    // only picks tower up if it is able to be picked up
    public int PickupTower(Coordinate c)
    {
        Coordinate cPos = c.GetChunkPos();
        BaseTower tower = ChunkManager.instance.GetHexAt(c).GetTower();
        if (tower != null && tower.IsRetrievable())
        {
            ChunkManager.instance.GetChunkAt(cPos).PickupTower(c);
            tower.transform.parent = savedTowers;
            tower.transform.localPosition = Vector3.zero;
            unplacedTowers.Add(tower);
            return unplacedTowers.Count - 1;
        }
        print("Error: tower unable to be picked up");
        return -1;
    }

    public bool PlaceTower(Coordinate c, BaseTower t)
    {
        if (ChunkManager.instance.IsPosLoaded(c))
        {
            if (unplacedTowers.Contains(t))
            {
                bool success = ChunkManager.instance.PlaceTower(c, t);
                if (success)
                {
                    unplacedTowers.Remove(t);
                    t.SetPos(c);
                }
                return success;
            }
        }
        else
        {
            print("Error: coordinate for tower creation isn't loaded");
        }
        return false;
    }

    public void DeleteUnplacedTower(int index)
    {
        if(index > 0 && index < unplacedTowers.Count)
        {
            BaseTower t = unplacedTowers[index];
            Destroy(t.gameObject);
            unplacedTowers.RemoveAt(index);
        }
    }

    public void DeleteUnplacedTower(BaseTower t)
    {
        int i = unplacedTowers.IndexOf(t);
        if (i != -1)
        {
            Destroy(t.gameObject);
            unplacedTowers.RemoveAt(i);
        }
    }

    public void PlaceRecentTower(Coordinate c)
    {
        if(unplacedTowers.Count != 0)
        {
            BaseTower t = unplacedTowers[unplacedTowers.Count - 1];
            PlaceTower(c, t);
        }
        else
        {
            print("Error: no towers in unplaced towers");
        }
    }

    public void DestroyTower(Coordinate c)
    {
        Coordinate cPos = c.GetChunkPos();
        ChunkManager.instance.GetChunkAt(c).DestroyTower(cPos);
    }

    public BaseTower GetUnplacedTower(int i) { return unplacedTowers[i]; }
    public int GetUnplacedTowerID(BaseTower t)
    {
        for(int i = 0; i < unplacedTowers.Count; i++)
        {
            if (unplacedTowers[i] == t)
                return i;
        }
        return -1;
    }
}
