using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public const float hexWidth = 1.732051f; // sqrt(3)
    public const float hexHeight = 2f;
    public const int chunkSize = 16;

    public static ChunkManager instance;

    public Transform chunksT;

    public GameObject hexPrefab;

    public List<Chunk> loadedChunks = new List<Chunk>();

    private float cTime = 0f;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        for (int i = -2; i <= 1; i++)
            for (int j = -2; j <= 1; j++)
                LoadChunk(i, j);
    }

    void Update()
    {
        if(Time.fixedTime - cTime > 0.05f)
        {
            foreach (Chunk lC in loadedChunks)
                lC.Tick();
            cTime = Time.fixedTime;
        }
    }

    // might eventually be not just 0,0
    public Coordinate GetBaseLocation()
    {
        Coordinate c = new Coordinate(0, 0);
        return c;
    }

    public bool IsHexOccupied(int x, int z)
    {
        return IsHexOccupied(new Coordinate(x, z));
    }
    public bool IsHexOccupied(Coordinate c)
    {
        return GetHexAt(c.x, c.z).IsOccupied();
    }

    public void CreateTower(int x, int z, int towerID, string towerName, object[] args)
    {
        CreateTower(new Coordinate(x, z), towerID, towerName, args);
    }
    public void CreateTower(Coordinate c, int towerID, string towerName, object[] args)
    {
        Coordinate chunkPos = c.GetChunkPos();
        if(IsChunkLoaded(chunkPos))
        {
            Chunk chunk = GetChunkAt(chunkPos);
            chunk.CreateTower(c, towerID, towerName, args);
        }
    }

    public HexInfo GetHexAt(int x, int z)
    {
        return GetHexAt(new Coordinate(x, z));
    }
    public HexInfo GetHexAt(Coordinate c)
    {
        Coordinate chunkPos = c.GetChunkPos();
        if (IsPosLoaded(c.x, c.z))
        {
            Chunk hexChunk = GetChunkAt(chunkPos);
            return hexChunk.GetHex(c.x, c.z);
        }
        else
        {
            // chunk isn't loaded, need to decide if we should
            // generate chunk or return null
        }
        return null;
    }

    // returns true if position x, z's chunk is loaded
    public bool IsPosLoaded(int x, int z)
    {
        return IsPosLoaded(new Coordinate(x, z));
    }
    public bool IsPosLoaded(Coordinate c)
    {
        Coordinate chunkPos = c.GetChunkPos();
        return IsChunkLoaded(chunkPos);
    }

    public bool IsChunkLoaded(int chunkX, int chunkZ)
    {
        return IsChunkLoaded(new Coordinate(chunkX, chunkZ));
    }
    public bool IsChunkLoaded(Coordinate c)
    {
        foreach (Chunk chunk in loadedChunks)
            if (chunk.GetChunkX() == c.x && chunk.GetChunkZ() == c.z)
                return true;
        return false;
    }

    public Chunk GetChunkAt(int chunkX, int chunkZ)
    {
        return GetChunkAt(new Coordinate(chunkX, chunkZ));
    }
    public Chunk GetChunkAt(Coordinate c)
    {
        for (int i = 0; i < loadedChunks.Count; i++)
            if (loadedChunks[i].GetChunkX() == c.x && loadedChunks[i].GetChunkZ() == c.z)
                return loadedChunks[i];
        return null;
    }

    // If chunk isn't already loaded, creates chunk object
    // initializes and builds the chunk, adds to loaded chunks
    public void LoadChunk(int chunkX, int chunkZ)
    {
        if(!IsChunkLoaded(chunkX, chunkZ))
        {
            Chunk newChunk = new Chunk(chunkX, chunkZ);
            loadedChunks.Add(newChunk);
        }
        else
            print("Error: trying to load chunk that already exists at x:" + chunkX + " z:" + chunkZ);
    }


    // Helper for the Chunk class, should only be called by Chunk
    // Creates the objects for a chunk into the world
    // returns those objects for the Chunk object to keep track of
    public HexInfo[,] Chunk_BuildChunk(int chunkX, int chunkZ)
    {
        HexInfo[,] hexes = new HexInfo[chunkSize, chunkSize];
        GameObject newChunk = new GameObject();
        newChunk.name = "C:" + chunkX + "," + chunkZ;
        newChunk.transform.parent = chunksT;
        
        Coordinate chunkPos = new Coordinate(chunkX, chunkZ);
        chunkPos = chunkPos.GetWorldPos();
        Vector3 worldPos = chunkPos.GetGamePos();

        newChunk.transform.position = worldPos;

        for(int x = 0; x < chunkSize; x++)
        {
            for(int z = 0; z < chunkSize; z++)
            {
                GameObject tile = Instantiate(hexPrefab, newChunk.transform);
                tile.transform.position = chunkPos.GetGamePos();
                tile.transform.localScale *= 2; // base radius is 0.5, so we double for a better radius of 1
                tile.name = "RP:" + chunkPos.x + "," + chunkPos.z;

                HexInfo info = tile.GetComponent<HexInfo>();
                info.x = chunkPos.x;
                info.z = chunkPos.z;

                hexes[x, z] = info;
                chunkPos.z += 1;
            }
            chunkPos.x += 1;
            chunkPos.z -= chunkSize;
        }

        return hexes;
    }
    
}