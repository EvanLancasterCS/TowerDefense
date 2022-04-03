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

    // returns index in unplaced array
    public int CreateTower(int towerID)
    {
        string towerName = TowerPresets.GetTowerName(towerID);
        object[] towerArgs = TowerPresets.GetTowerArgs(towerID);
        int towerQual = TowerPresets.GetTowerQuality(towerID);

        GameObject newTower = GameObject.Instantiate(TowerManager.instance.towerPrefab, savedTowers);
        newTower.transform.localPosition = Vector3.zero;
        BaseTower tower = null;
        BaseTower.TowerType type = TowerPresets.GetTowerType(towerID);

        // Add appropriate behaviour for tower
        if (type == BaseTower.TowerType.Projectile)
            tower = newTower.AddComponent<ProjectileTower>();
        else if (type == BaseTower.TowerType.Home)
            tower = newTower.AddComponent<HomeTower>();
        else
            print("Error: Type isn't any tower type"); // shouldn't ever be here unless you do some weird casting

        tower.SetTower(new Coordinate(5000, 5000), towerID, towerQual, towerName, towerArgs);

        // Request visual from resources and place in object
        GameObject visualPrefab = ResourceLoader.instance.GetTowerModel(towerID);
        GameObject.Instantiate(visualPrefab, newTower.transform);

        unplacedTowers.Add(tower);
        return unplacedTowers.Count - 1;
    }

    public void PlaceTower(Coordinate c, BaseTower t)
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
            }
        }
        else
        {
            print("Error: coordinate for tower creation isn't loaded");
        }
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
}
