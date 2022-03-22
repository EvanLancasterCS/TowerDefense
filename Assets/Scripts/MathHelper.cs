using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    // Converts a world array coordinate to a chunk coordinate
    public static Coordinate WorldToChunk(Coordinate c)
    {
        int nX, nZ;

        if (c.x < 0)
            nX = ((c.x + 1) / ChunkManager.chunkSize);
        else
            nX = (c.x / ChunkManager.chunkSize);

        if (c.z < 0)
            nZ = ((c.z + 1) / ChunkManager.chunkSize);
        else
            nZ = (c.z / ChunkManager.chunkSize);

        if (c.x < 0)
            nX -= 1;
        if (c.z < 0)
            nZ -= 1;
        return new Coordinate(nX, nZ);
    }
    public static Coordinate WorldToChunk(int x, int z)
    {
        return WorldToChunk(new Coordinate(x, z));
    }

    // Converts a chunk coordinate to world array coordinate
    public static Coordinate ChunkToWorld(Coordinate c)
    {
        int nX, nZ;
        nX = c.x * ChunkManager.chunkSize;
        nZ = c.z * ChunkManager.chunkSize;
        return new Coordinate(nX, nZ);
    }
    public static Coordinate ChunkToWorld(int x, int z)
    {
        return ChunkToWorld(new Coordinate(x, z));
    }

    // Converts a world array coordinate to a game world coordinate
    public static Vector3 WorldToGame(Coordinate c)
    {
        bool even = c.z % 2 == 0;
        float shift = even ? 1 : 0;
        Vector3 pos = new Vector3((float)c.x * ChunkManager.hexHeight + shift, 0, (float)c.z * ChunkManager.hexWidth);
        return pos;
    }
    public static Vector3 WorldToGame(int x, int z)
    {
        return WorldToGame(new Coordinate(x, z));
    }
}
