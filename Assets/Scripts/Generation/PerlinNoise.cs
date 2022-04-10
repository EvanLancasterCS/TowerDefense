using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static int seed = 9993999;
    public static MapPackage GenerateRocksMap(int w, int h, float scale, float wallWeight, float rockChance, int clearDistance, int tendrilDistance, int tendrilEndRadius)
    {
        Texture2D texture = new Texture2D(w, h);
        int centerX = (w / 2);
        int centerY = (h / 2);

        System.Random rand = new System.Random();
        seed = rand.Next();

        ApplyNoise(texture, scale);

        ForceColors(texture, wallWeight);

        HexagonalCellular(texture);

        OverlayRandomRocks(texture, rockChance);

        ClearSpot(texture, centerX, centerY, clearDistance);

        Coordinate[] tendrilEnds = CreateHallways(texture, centerX, centerY, tendrilDistance, tendrilEndRadius);

        texture.Apply();
        MapPackage map = new MapPackage(texture, tendrilEnds);

        return map;
    }

    private static void ApplyNoise(Texture2D t, float scale)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        int w = t.width;
        int h = t.height;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float val = (rand.Next(0, 100) / 100f); //Random.Range(0f, 1f);
                Color c = CalculateColor(x, y, w, h, scale);
                t.SetPixel(x, y, c);
            }
        }
    }

    private static void ForceColors(Texture2D t, float wallWeight)
    {
        int w = t.width;
        int h = t.height;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float weight = t.GetPixel(x, y).r;
                if (weight < wallWeight)
                    t.SetPixel(x, y, Color.black);
                else
                    t.SetPixel(x, y, Color.white);
            }
        }
    }

    private static void HexagonalCellular(Texture2D t)
    {
        int w = t.width;
        int h = t.height;
        for(int x = 0; x < w; x++)
        {
            for(int y = 0; y < h; y++)
            {
                Coordinate pos = new Coordinate(x, y);
                Coordinate[] neighbors = pos.GetNeighbors();
                int numNeighborWalls = 0;
                foreach (Coordinate c in neighbors)
                {
                    if (c.x >= 0 && c.x < w && c.z >= 0 && c.z < h)
                    {
                        float neighborEval = t.GetPixel(c.x, c.z).r;
                        if (neighborEval == 0)
                            numNeighborWalls += 1;
                    }
                }
                if (numNeighborWalls >= 4)
                    t.SetPixel(x, y, Color.black);
                else
                    t.SetPixel(x, y, Color.white);

            }
        }
    }

    private static void OverlayRandomRocks(Texture2D t, float chance)
    {
        System.Random rand = new System.Random(seed.GetHashCode());
        int w = t.width;
        int h = t.height;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float num = rand.Next(0, 100) / 100f;
                if (num < chance)
                {
                    float val = t.GetPixel(x, y).r;
                    if (val == 1)
                        t.SetPixel(x, y, Color.black);
                    else
                        t.SetPixel(x, y, Color.white);
                }
            }
        }
    }


    // clears distance around a given spot hexagonally
    private static void ClearSpot(Texture2D t, int cx, int cy, int distance)
    {
        List<Coordinate> open = new List<Coordinate>();
        List<Coordinate> next = new List<Coordinate>();
        List<Coordinate> visited = new List<Coordinate>();
        open.Add(new Coordinate(cx, cy));
        for(int i = 0; i < distance; i++)
        {
            for(int j = open.Count - 1; j >= 0; j--)
            {
                Coordinate c = open[j];
                Coordinate[] neighbors = c.GetNeighbors();
                foreach(Coordinate n in neighbors)
                    if (!visited.Contains(n))
                        next.Add(n);

                t.SetPixel(c.x, c.z, Color.white);
            }
            // exchange open and next array
            open.Clear();
            for (int j = 0; j < next.Count; j++)
                open.Add(next[j]);
            next.Clear();
        }
    }

    // creates 4 tendrils from home to distance in all cardinal directions
    // returns the positions of the tendril ends
    private static Coordinate[] CreateHallways(Texture2D t, int cx, int cy, int distance, int endRadius)
    {
        System.Random rand = new System.Random(seed.GetHashCode());
        Coordinate[] ends = new Coordinate[4];
        int currIndex = 0;
        for (int xDiff = -1; xDiff <= 1; xDiff++)
        {
            for (int yDiff = -1; yDiff <= 1; yDiff++)
            {
                if (xDiff != 0 && yDiff != 0)
                    continue; // only do non-diagonals
                if (xDiff == 0 && yDiff == 0)
                    continue;

                int rX = 0;
                int rY = 0;
                for(int d = 1; d < distance; d++)
                {
                    int xPos = cx + (xDiff * d) + rX;
                    int yPos = cy + (yDiff * d) + rY;

                    Coordinate pos = new Coordinate(xPos, yPos);
                    Coordinate[] neighbors = pos.GetNeighbors();
                    foreach (Coordinate c in neighbors)
                    { 
                        t.SetPixel(c.x, c.z, Color.white);
                    }
                    t.SetPixel(pos.x, pos.z, Color.white);

                    if (xDiff == 0)
                        rX += rand.Next(-1, 2);
                    else if (yDiff == 0)
                        rY += rand.Next(-1, 2);
                }

                int finalPosX = cx + (xDiff * (distance - 1)) + rX;
                int finalPosY = cy + (yDiff * (distance - 1)) + rY;
                ClearSpot(t, finalPosX, finalPosY, endRadius);
                ends[currIndex] = new Coordinate(finalPosX, finalPosY) - new Coordinate(cx, cy);
                currIndex += 1;
            }
        }

        return ends;
    }

    public static Color CalculateColor(int x, int y, int w, int h, float scale)
    {
        int seedShift = seed.GetHashCode() / 10000;
        float xPerlin = scale * (float)x / w + seedShift;
        float yPerlin = scale * (float)y / h + seedShift;

        float sample = Mathf.PerlinNoise(xPerlin, yPerlin);
        return new Color(sample, sample, sample);
    }

    // helper for returning results of map generation
    public class MapPackage
    {
        private Texture2D map;
        private Coordinate[] tendrilEnds;

        public MapPackage(Texture2D _map, Coordinate[] _tendrilEnds)
        {
            map = _map;
            tendrilEnds = _tendrilEnds;
        }

        public Texture2D GetMap() => map;
        public Coordinate[] GetTendrilEnds() => tendrilEnds;
    }
}
