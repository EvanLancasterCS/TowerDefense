using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private int chunkX, chunkZ;
    public HexInfo[,] myHexes;
    public List<BaseTower> myTowers = new List<BaseTower>();

    public Chunk(int cX, int cZ)
    {
        chunkX = cX;
        chunkZ = cZ;
        myHexes = new HexInfo[ChunkManager.chunkSize, ChunkManager.chunkSize];

        bool success = Load();
        if(!success)
            Debug.Log("Failed to initialize chunk at x:" + cX + " z:" + cZ);
        
    }

    public void Tick()
    {
        foreach (BaseTower t in myTowers)
        {
            t.Tick();
        }
    }

    public bool IsHexOccupied(Coordinate c)
    {
        return myHexes[c.x, c.z].IsOccupied();
    }

    public bool PlaceTower(Coordinate c, BaseTower t)
    {
        HexInfo hex = GetHex(c);
        if(hex != null)
        {
            Transform tTrans = t.transform;
            tTrans.parent = hex.transform;
            tTrans.localPosition = Vector3.zero;

            myTowers.Add(t);
            return hex.SetTower(t);
        }
        return false;
    }

    /*
    // Creates tower at x, z, adds to myTowers
    public void CreateTower(int x, int z, int towerID, int qual, string towerName, object[] args)
    {
        CreateTower(new Coordinate(x, z), towerID, qual, towerName, args);
    }
    public void CreateTower(Coordinate c, int towerID, int qual, string towerName, object[] args)
    {
        HexInfo hex = GetHex(c);
        if (hex != null)
        {
            GameObject newTower = GameObject.Instantiate(TowerManager.instance.towerPrefab, hex.transform);
            newTower.transform.localPosition = Vector3.zero;
            BaseTower tower = null;
            BaseTower.TowerType type = TowerPresets.GetTowerType(towerID);
            
            // Add appropriate behaviour for tower
            if (type == BaseTower.TowerType.Projectile)
                tower = newTower.AddComponent<ProjectileTower>();
            else if (type == BaseTower.TowerType.Home)
                tower = newTower.AddComponent<HomeTower>();

            // Request visual from resources and place in object
            GameObject visualPrefab = ResourceLoader.instance.GetTowerModel(towerID);
            GameObject.Instantiate(visualPrefab, newTower.transform);


            if (tower != null)
            {
                tower.SetTower(c, towerID, qual, towerName, args);
                myTowers.Add(tower);
                GetHex(c).SetTower(tower);
            }
            else
            {
                Debug.Log("Error: tower type invalid");
            }
        }
    }*/

    public void DestroyTower(Coordinate c)
    {
        HexInfo hex = GetHex(c);
        if(hex != null)
        {
            BaseTower t = hex.GetTower();
            int i = myTowers.IndexOf(t);
            if (t != null && i != -1)
            {
                GameObject tObj = t.gameObject;
                GameObject.Destroy(tObj);
                myTowers.RemoveAt(i);
                hex.SetTower(null);
            }
            else
            {
                Debug.Log("Error: hex does not have a tower for DestroyTower, or tower was not found in this chunk");
            }
        }
        else
        {
            Debug.Log("Error: hex not found for DestroyTower");
        }
    }

    // Returns the hex at x, z; turns into local coords. returns null if not in this chunk
    public HexInfo GetHex(int x, int z)
    {
        return GetHex(new Coordinate(x, z));
    }
    public HexInfo GetHex(Coordinate c)
    {
        int cXW = chunkX * ChunkManager.chunkSize;
        int cZW = chunkZ * ChunkManager.chunkSize;
        Coordinate localCoord = new Coordinate(c.x - cXW, c.z - cZW);
        if(localCoord.x < 0 || localCoord.x >= ChunkManager.chunkSize ||
            localCoord.z < 0 || localCoord.z >= ChunkManager.chunkSize)
        {
            Debug.Log("Error: hex out of bounds for chunk at x:" + chunkX + ",z:" + chunkZ);
            return null;
        }
        return myHexes[localCoord.x, localCoord.z];
    }

    public int GetChunkX() { return chunkX; }
    public int GetChunkZ() { return chunkZ; }

    // requests the ChunkManager to build chunk at coordinates
    private bool Load()
    {
        HexInfo[,] hexes = ChunkManager.instance.Chunk_BuildChunk(chunkX, chunkZ);
        myHexes = hexes;
        return true;
    }

    // requests the ChunkManager to unload / destroy this chunk
    // returns true if successful
    // this object should be destroyed afterwards
    public bool Unload()
    {

        return true;
    }
}
