using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TowerPresets
{
    
    private static TowerHelper[] presets =
    {
        new TowerHelper(0, 1, BaseTower.TowerType.Home, "Home Base", null),
        new TowerHelper(1, 1, BaseTower.TowerType.Projectile, "Archer", new object[]{1, 4, 0f, 0f, 10f, 2f}),
        new TowerHelper(2, 1, BaseTower.TowerType.Projectile, "Shotgun", new object[]{8, 1, 25f, 0f, 10f, 1f}),
        new TowerHelper(3, 1, BaseTower.TowerType.Projectile, "Arty", new object[]{1, 8, 0f, 0f, 20f, 1f })
    };

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
