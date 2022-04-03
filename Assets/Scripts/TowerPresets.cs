using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TowerPresets
{
    
    private static TowerHelper[] presets =
    {
        new TowerHelper(0, 1, BaseTower.TowerType.Home, "Home Base", null),
        new TowerHelper(1, 1, BaseTower.TowerType.Projectile, "Archer", new object[]{1, 4, 0f, 0f, 10f, 2f}),
        new TowerHelper(2, 1, BaseTower.TowerType.Projectile, "Shotgun", new object[]{8, 1, 25f, 0f, 10f, 1f})
    };

    private static int[] towerProbabilities =
    {
        0,
        1,
        1
    };
    private static int totalProbability = -1; // dont set this to anything

    // idk the optimal way to roll but this works for small things i guess lol
    public static int RollForTower()
    {
        if(totalProbability == -1)
            for (int i = 0; i < towerProbabilities.Length; i++)
                totalProbability += towerProbabilities[i];

        int rand = Random.Range(1, totalProbability + 1);
        int count = 0;
        for(int i = 0; i < towerProbabilities.Length; i++)
        {
            if(count == rand || (count < rand && count + towerProbabilities[i] >= rand))
                return i;
            count += towerProbabilities[i];
        }
        Debug.Log("error: rolling for tower");
        return -1;
    }

    public static BaseTower.TowerType GetTowerType(int id) { return presets[id].type; }
    public static string GetTowerName(int id) { return presets[id].name; }
    public static object[] GetTowerArgs(int id) { return presets[id].args; }
    public static int GetTowerQuality(int id) { return presets[id].quality; }
    
    private class TowerHelper
    {
        public int id;
        public BaseTower.TowerType type;
        public int quality;
        public string name;
        public object[] args;

        public TowerHelper(int _id, int _quality, BaseTower.TowerType _type, string _name, object[] _args)
        {
            id = _id;
            type = _type;
            quality = _quality;
            name = _name;
            args = _args;
        }
    }
}
