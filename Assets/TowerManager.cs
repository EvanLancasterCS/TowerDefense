using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;
    public GameObject towerPrefab;

    public GameObject projectilePrefab;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        
    }

    public void CreateTower(Coordinate c, int towerID)
    {
        string towerName = TowerPresets.GetTowerName(towerID);
        object[] towerArgs = TowerPresets.GetTowerArgs(towerID);

        if(ChunkManager.instance.IsPosLoaded(c))
        {
            ChunkManager.instance.CreateTower(c, towerID, towerName, towerArgs);
        }
        else
        {
            print("Error: coordinate for tower creation isn't loaded");
        }
    }
}
