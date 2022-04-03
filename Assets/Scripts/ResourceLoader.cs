using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public static ResourceLoader instance;
    public GameObject[] TowerModelsByID;
    public GameObject[] EnemyModelsByID;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject GetTowerModel(int towerID)
    {
        if(towerID < 0 || towerID > TowerModelsByID.Length)
        {
            print("Model not found");
            return null;
        }

        return TowerModelsByID[towerID];
    }

    public GameObject GetEnemyModel(int enemyID)
    {
        if (enemyID < 0 || enemyID > EnemyModelsByID.Length)
        {
            print("Model not found");
            return null;
        }

        return EnemyModelsByID[enemyID];
    }
}
