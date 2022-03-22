using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TowerPresets
{
    // p[i][0] = towerID
    // p[i][1] = towerType, based on enum in BaseTower
    // p[i][2] = towerName
    // p[i][2-end] = towerArgs
    private static TowerHelper[] presets =
    {
        new TowerHelper(0, BaseTower.TowerType.Home, "Home Base", null),
        new TowerHelper(1, BaseTower.TowerType.Projectile, "Archer", new object[]{1, 4, 0f, 0f, 10f, 2f}),
        new TowerHelper(1, BaseTower.TowerType.Projectile, "Shotgun", new object[]{8, 1, 25f, 0f, 10f, 1f})
    };

    public static BaseTower.TowerType GetTowerType(int id) { return presets[id].type; }
    public static string GetTowerName(int id) { return presets[id].name; }
    public static object[] GetTowerArgs(int id) { return presets[id].args; }
    
    private class TowerHelper
    {
        public int id;
        public BaseTower.TowerType type;
        public string name;
        public object[] args;

        public TowerHelper(int _id, BaseTower.TowerType _type, string _name, object[] _args)
        {
            id = _id;
            type = _type;
            name = _name;
            args = _args;
        }
    }
}
