using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate
{
    // even/odd diffs are required for the representation of the board.
    // i chose do normal x, z 2 dimensional representation, and i chose
    // to offset the even z's. therefore, we need to treat even and odd
    // rows differently, and these are the numbers for those movements,
    // starting at EAST and moving clockwise.
    // would not recommend trying to obtain neighbors without using
    // the functions in this class, but can if necessary for some reason
    private static Coordinate[] even_differences =
    {
        new Coordinate(1, 0), new Coordinate(1, -1), new Coordinate(0, -1),
        new Coordinate(-1, 0), new Coordinate(0, 1), new Coordinate(1, 1)
    };
    private static Coordinate[] odd_differences =
    {
        new Coordinate(1, 0), new Coordinate(0, -1), new Coordinate(-1, -1),
        new Coordinate(-1, 0), new Coordinate(-1, 1), new Coordinate(0, 1)
    };

    public int x, z;

    public Coordinate()
    {
        x = 0;
        z = 0;
    }

    public Coordinate(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    // Should only be used if this is a world position
    public Coordinate GetChunkPos() { return MathHelper.WorldToChunk(this); }

    // Should only be used if this is a chunk position
    public Coordinate GetWorldPos() { return MathHelper.ChunkToWorld(this); }

    // Should only be used if this is a world position
    public Vector3 GetGamePos() { return MathHelper.WorldToGame(this); }

    // returns a list of this coord's hexagonal neighbors
    // doesn't guarantee they're loaded
    public Coordinate[] GetNeighbors()
    {
        Coordinate[] diffs = null;
        Coordinate[] neighbors = new Coordinate[6];
        if (z % 2 == 0)
            diffs = even_differences;
        else
            diffs = odd_differences;

        for (int i = 0; i < 6; i++)
            neighbors[i] = (this + diffs[i]);
        
        return neighbors;
    }

    public bool IsNeighbor(Coordinate b)
    {
        Coordinate[] coords = GetNeighbors();
        for (int i = 0; i < 6; i++)
            if (coords[i] == b) 
                return true;
        return false;
    }

    public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.x + b.x, a.z + b.z);
    public static Coordinate operator -(Coordinate a, Coordinate b) => new Coordinate(a.x - b.x, a.z - b.z);

    public static bool operator ==(Coordinate a, Coordinate b) => a.x == b.x && a.z == b.z;
    public static bool operator !=(Coordinate a, Coordinate b) => a.x != b.x || a.z != b.z;

    public override bool Equals(object obj)
    {
        if (!(obj is Coordinate)) return false;
        Coordinate c = (Coordinate)obj;
        return this.x == c.x && this.z == c.z;
    }

    private int ReverseBits(int num)
    {
        string binary = Convert.ToString(num, 2).PadLeft(32, '0');
        char[] arr = binary.ToCharArray();
        Array.Reverse(arr);
        int reversed = Convert.ToInt32(new string(arr), 2);
        return reversed;
    }

    public override int GetHashCode()
    {
        return ReverseBits(x) ^ (z << 1);
    }

    public override string ToString()
    {
        return "(" + x + ", " + z + ")";
    }
}
