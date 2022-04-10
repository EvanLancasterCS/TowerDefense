using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerationManager
{
    public static PerlinNoise.MapPackage currentMap = null;

    private static void SetupRocks(Texture2D perlin, int perlinSize, int xVariation, int zVariation)
    {
        for (int x = -xVariation; x <= xVariation; x++)
        {
            for (int z = -zVariation; z <= zVariation; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                //float roll = Random.Range(0f, 1f);
                //bool isRock = roll < rockChance;
                int xPerlin = (int)(perlinSize * ((x + xVariation) / (2f * xVariation)));
                int zPerlin = (int)(perlinSize * ((z + zVariation) / (2f * zVariation)));
                float height = perlin.GetPixel(xPerlin, zPerlin).r;
                bool isRock = height == 0;

                if (isRock)
                {
                    TowerManager.instance.CreateTower(6); // 6 is rock tower ID
                    TowerManager.instance.PlaceRecentTower(new Coordinate(x, z));
                }
            }
        }
    }

    // returns enemy spawn point locations
    public static Coordinate[] GenerateMap()
    {
        // creates player home tower
        int towerIndex = TowerManager.instance.CreateTower(0);
        BaseTower tower = TowerManager.instance.GetUnplacedTower(towerIndex);
        TowerManager.instance.PlaceTower(new Coordinate(0, 0), tower);

        // generates perlin noise map, enemy spawn locations, sets up rocks

        int variation = 30; // needs to be an even number because i don't want to deal with shifting hexagons; size / 2
        int xVariation = variation;
        int zVariation = variation;
        int perlinSize = variation * 2; // size of the perlin texture
        float perlinScale = 20f; // scale of the perlin noise
        float wallWeight = 0.65f; // threshold for perlin noise to be turned into a wall
        float extraRockChance = 0.1f; // chance for extra random rock per tile

        int clearFromHomeDistance = 3; // clear space around home

        int tendrilDistance = 15; // distance each tendril should go out
        int tendrilEndRadius = 3; // clear space around each tendril end

        PerlinNoise.MapPackage map = PerlinNoise.GenerateRocksMap(perlinSize, perlinSize, perlinScale, wallWeight, extraRockChance, clearFromHomeDistance, tendrilDistance, tendrilEndRadius);
        Texture2D perlin = map.GetMap();

        Coordinate[] tendrilEnds = map.GetTendrilEnds();

        SetupRocks(perlin, perlinSize, xVariation, zVariation);

        GenerationManager.currentMap = map;
        return tendrilEnds;
    }
}
