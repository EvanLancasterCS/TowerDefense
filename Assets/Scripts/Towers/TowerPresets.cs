using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* full preset example projectile
 new TowerHelper(1, 1, true, BaseTower.TowerType.Projectile, "Archer", 
            new object[]{
                1, // num projectiles
                20, // damage per projectile
                0f, // spread
                0f, // burst time
                10f, // range
                2f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[] 
                { 
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.explosive, 
                        new object[] 
                        { 
                            5, // explosive damage
                            2f // explosion range
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            5 // homing strength
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1 // number of pierces allowed
                        }
                    )
                } 
            } 
        ), 
*/

public static class TowerPresets
{
    // [0] -> home base. 
    // [1] -> archer.       [2] -> shotgun.         [3] -> arty.            [4] -> machine gun.      [5] -> burst.           [6] -> rock.
    // [7] -> sharpshooter. [8] -> blazer.          [9] -> marksman.        [10] -> auto-mg.        [11] -> power-burst.
    //  [12] -> Blast Archer [13] -> Pump Shotgun   [14] -> Modular Arty    [15] -> Enhanced-MG     [16] -> Precision Burst
    public const int NUM_PRESETS = 12;
    private static TowerHelper[] presets =
    {
        new TowerHelper(0, 1, -1, false, BaseTower.TowerType.Home, "Home Base", null),
        new TowerHelper(1, 1, 7, true, BaseTower.TowerType.Projectile, "Archer", 
            new object[]{
                1, // num projectiles
                20, // damage per projectile
                0f, // spread
                0f, // burst time
                10f, // range
                2f, // fire rate
                // v projectile effects
                null
            }
        ),
        new TowerHelper(2, 1, 8, true, BaseTower.TowerType.Projectile, "Shotgun", 
            new object[]{
                8, // num projectiles
                10, // damage per projectile
                25f, //spread
                0f, // burst time
                10f, // range
                1f, // fire rate
                null // projectile effects
            }
        ),
        new TowerHelper(3, 1, 9, true, BaseTower.TowerType.Projectile, "Arty", 
            new object[]{
                1, // num projectiles
                70, // damage per projectile
                0f, // spread
                0f, // burst time
                12f, // range
                0.5f, // fire rate
                null // projectile effects
            }
        ),
        new TowerHelper(4, 1, 10, true, BaseTower.TowerType.Projectile, "Machine-gun", 
            new object[]{
                1, // num projectiles
                10, // damage per projectile
                0f, //spread
                0f, //burst time
                10f, // range
                5f, // fire rate
                null // projectile effects
            }
        ),
        new TowerHelper(5, 1, 11, true, BaseTower.TowerType.Projectile, "Burst", 
            new object[]{
                3, // num projectiles
                15, // damage per projectile
                10f, //spread
                0.1f, // burst time
                10f, // range
                1f, // fire rate
                null // projectile effects
            }
        ),
        new TowerHelper(6, 0, -1, false, BaseTower.TowerType.Wall, "Rock", new object[]{100, false}),
        new TowerHelper(7, 2, 12, true, BaseTower.TowerType.Projectile, "Sharpshooter",
            new object[]{
                1, // num projectiles
                25, // damage per projectile
                0f, // spread
                0f, // burst time
                12f, // range
                2f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1 // number of pierces allowed
                        }
                    )
                }
            }
        ),
        new TowerHelper(8, 2, 13, true, BaseTower.TowerType.Projectile, "Blazer",
            new object[]{
                10, // num projectiles
                2, // damage per projectile
                20f, // spread
                0f, // burst time
                6f, // range
                1f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.explosive,
                        new object[]
                        {
                            10, // explosive damage
                            2f // explosion range
                        }
                    )
                }
            }
        ),
        new TowerHelper(9, 2, 14, true, BaseTower.TowerType.Projectile, "Marksman",
            new object[]{
                1, // num projectiles
                100, // damage per projectile
                0f, // spread
                0f, // burst time
                15f, // range
                0.5f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            1 // homing strength
                        }
                    )
                }
            }
        ),
        new TowerHelper(10, 2, 15, true, BaseTower.TowerType.Projectile, "Auto-MG",
            new object[]{
                1, // num projectiles
                12, // damage per projectile
                0f, // spread
                0f, // burst time
                10f, // range
                8f, // fire rate
                // v projectile effects
                null
            }
        ),
        new TowerHelper(11, 2, 16, true, BaseTower.TowerType.Projectile, "Power-burst",
            new object[]{
                3, // num projectiles
                12, // damage per projectile
                10f, // spread
                0.1f, // burst time
                10f, // range
                1f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1 // num pierce
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            1 // homing strength
                        }
                    )
                }
            }
        ),
        new TowerHelper(12, 3, -1, true, BaseTower.TowerType.Projectile, "Blast Archer",
            new object[]{
                1, // num projectiles
                5, // damage per projectile
                0f, // spread
                0f, // burst time
                12f, // range
                2f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.explosive,
                        new object[]
                        {
                            50, // explosive damage
                            4f // explosion range
                        }
                    )
                }
            }
        ),
        new TowerHelper(13, 3, -1, true, BaseTower.TowerType.Projectile, "Pump Shotgun",
            new object[]{
                10, // num projectiles
                10, // damage per projectile
                10f, // spread
                0f, // burst time
                6f, // range
                1f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1, // num pierce
                        }
                    )
                }
            }
        ),
        new TowerHelper(14, 3, -1, true, BaseTower.TowerType.Projectile, "Modular Arty",
            new object[]{
                1, // num projectiles
                150, // damage per projectile
                0f, // spread
                0f, // burst time
                15f, // range
                0.5f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            1 // homing strength
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1 // num pierce
                        }
                    )
                }
            }
        ),
        new TowerHelper(15, 3, -1, true, BaseTower.TowerType.Projectile, "Enhanced-MG",
            new object[]{
                1, // num projectiles
                10, // damage per projectile
                0f, // spread
                0f, // burst time
                10f, // range
                10f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            1 // homing strength
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.explosive,
                        new object[]
                        {
                            5, // explosive damage
                            2f // explosion range
                        }
                    )
                }
            }
        ),
        new TowerHelper(16, 3, -1, true, BaseTower.TowerType.Projectile, "Precision-burst",
            new object[]{
                3, // num projectiles
                50, // damage per projectile
                3f, // spread
                0.1f, // burst time
                10f, // range
                1f, // fire rate
                // v projectile effects
                new ProjectileEffect.Helper[]
                {
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.piercing,
                        new object[]
                        {
                            1 // num pierce
                        }
                    ),
                    new ProjectileEffect.Helper(ProjectileEffect.EffectType.homing,
                        new object[]
                        {
                            1 // homing strength
                        }
                    )
                }
            }
        ),
    };

    private static int[] towerProbabilities =
    {
        0,
        1,
        1,
        1,
        1,
        1,
        0
    };
    private static int totalProbability = -1; // dont set this to anything

    // idk the optimal way to roll but this works for small things i guess lol
    public static int RollForTower()
    {
        if (totalProbability == -1)
        {
            totalProbability = 0;
            for (int i = 0; i < towerProbabilities.Length; i++)
            {
                totalProbability += towerProbabilities[i];
            }
        }

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

    public static void SetProbabilities(int[] probs)
    {
        towerProbabilities = probs;
        totalProbability = 0;
        for (int i = 0; i < towerProbabilities.Length; i++)
        {
            totalProbability += towerProbabilities[i];
        }
    }

    public static BaseTower.TowerType GetTowerType(int id) { return presets[id].type; }
    public static string GetTowerName(int id) { return presets[id].name; }
    public static object[] GetTowerArgs(int id) { return presets[id].args; }
    public static int GetTowerQuality(int id) { return presets[id].quality; }
    public static bool GetTowerRetrievable(int id) { return presets[id].retrievable; }
    public static int GetTowerUpgradeID(int id) { return presets[id].upgradeTower; }

    private class TowerHelper
    {
        public int id;
        public BaseTower.TowerType type;
        public int quality;
        public int upgradeTower;
        public bool retrievable;
        public string name;
        public object[] args;

        public TowerHelper(int _id, int _quality, int _upgradeTower, bool _retrievable, BaseTower.TowerType _type, string _name, object[] _args)
        {
            id = _id;
            type = _type;
            upgradeTower = _upgradeTower;
            quality = _quality;
            retrievable = _retrievable;
            name = _name;
            args = _args;
        }
    }
}
