using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;
    private List<List<Coordinate>> recentPaths = new List<List<Coordinate>>();
    private int maxRecent = 6;
    private void Awake()
    {
        instance = this;
    }

    public void ClearRecentPaths() { recentPaths.Clear(); }

    // returns result to the request manager
    public void GetPath(Coordinate start, Coordinate end)
    {
        StartCoroutine(RequestPath(start, end));
    }

    // uses the hexagonal map to find a path from A to B
    // uses min-heap with hash table to keep track of lowest score and track visited nodes
    // returns result to the request manager
    // ienumerator so we can delay to not have huge frame drop suddenly for large calculations
    public IEnumerator RequestPath(Coordinate start, Coordinate end)
    {
        for(int i = 0; i < recentPaths.Count; i++)
        {
            int length = recentPaths[i].Count;
            if (start == recentPaths[i][0] && end == recentPaths[i][length - 1])
            {
                RequestManager.instance.FinishedProcessingPath(PathCopy(recentPaths[i]), true);
                yield break;
            }
            else if (start == recentPaths[i][length - 1] && end == recentPaths[i][0])
            {
                print("need flip");
            }
        }
        
        Stopwatch s = new Stopwatch();
        s.Start();
        HashHeap openSet = new HashHeap(10000); // magical number that we shouldn't have more nodes than
        Hashtable closedSet = new Hashtable();//HashHeap closedSet = new HashHeap(10000);
        Node nstart = new Node(start, null);
        nstart.setScore(0, end);

        openSet.Add(nstart);

        List<Coordinate> path = null;
        bool found = false;

        while(!openSet.IsEmpty()) 
        {
            if (s.ElapsedMilliseconds > 5)
            {
                yield return null;
                s.Restart();
            }

            Node lowest = openSet.Pop();
            if (lowest.pos == end)
            {
                path = RebuildPath(lowest); // only runs once
                found = true;
                break;
            }


            Coordinate[] neighbors = lowest.pos.GetNeighbors();
            foreach (Coordinate c in neighbors)
            {
                if (lowest.from != null && lowest.from.pos == c) // ignore nodes that go backwards, backtracking never optimal
                    continue;
                if (!ChunkManager.instance.IsPosLoaded(c) || ChunkManager.instance.IsHexOccupied(c)) 
                    continue;
                if (closedSet.Contains(c.GetHashCode()))
                    continue;

                float tentG = lowest.gScore + 1; // constant movement distance across hexagons

                float g = float.MaxValue;
                Node n = null;
                if (openSet.Contains(c)) // O(1)
                {
                    n = openSet.HashGet(c); // O(1) hopefully
                    g = n.gScore;
                }

                if(tentG < g)
                {
                    if(n != null)
                    {
                        n.setScore(tentG, end);
                        n.from = lowest;
                        
                    }
                    else
                    {
                        n = new Node(c, lowest);
                        n.setScore(tentG, end);
                        openSet.Add(n); // O(logn) hopefully
                    }
                }
            }
            closedSet.Add(lowest.GetHashCode(), lowest);

            //yield return null;
        }
        yield return null;

        if (recentPaths.Count >= maxRecent)
            recentPaths.RemoveAt(0);

        if (path != null)
        {
            List<Coordinate> pathCopy = PathCopy(path);
            recentPaths.Add(pathCopy);
        }

        RequestManager.instance.FinishedProcessingPath(path, found);
    }

    private static List<Coordinate> PathCopy(List<Coordinate> path)
    {
        List<Coordinate> pathCopy = new List<Coordinate>();
        foreach (Coordinate c in path)
            pathCopy.Add(new Coordinate(c.x, c.z));
        return pathCopy;
    }

    private static List<Coordinate> RebuildPath(Node end)
    {
        List<Coordinate> path = new List<Coordinate>();
        while(end != null)
        {
            path.Add(end.pos);
            end = end.from;
        }
        path.Reverse();
        return path;
    }

    // helper class: min heap for openset, with hash table for checking duplicates
    // idk if this is the best way to do it, but should be good enough
    // some help from https://egorikas.com/max-and-min-heap-implementation-with-csharp/
    // since c# doesn't have a min heap structure i guess >:(
    private class HashHeap
    {
        Node[] nodes;
        Hashtable hash = new Hashtable();
        private int size;
        public HashHeap(int _size) { nodes = new Node[_size]; }

        public void Add(Node n)
        {
            if (size == nodes.Length) throw new System.IndexOutOfRangeException();

            if(Contains(n))
                return;

            hash.Add(n.GetHashCode(), n);

            nodes[size] = n;
            size++;

            RecalculateUp();
        }

        public Node Pop()
        {
            if (size == 0) throw new System.IndexOutOfRangeException();

            Node result = nodes[0];
            nodes[0] = nodes[size - 1];
            size--;

            hash.Remove(result.GetHashCode());

            RecalculateDown();
            return result;
        }

        public Node HashGet(Coordinate c)
        {
            return (Node)hash[c.GetHashCode()];
        }

        public bool Contains(Node n)
        {
            return hash.Contains(n.GetHashCode());
        }

        public bool Contains(Coordinate c)
        {
            return hash.Contains(c.GetHashCode());
        }

        private int GetLeftIndex(int i) => 2 * i + 1;
        private int GetRightIndex(int i) => 2 * i + 2;
        private int GetParentIndex(int i) => (i - 1) / 2;

        private bool HasLeftChild(int i) => GetLeftIndex(i) < size;
        private bool HasRightChild(int i) => GetRightIndex(i) < size;
        private bool IsRoot(int i) => i == 0;

        private Node GetLeftChild(int i) => nodes[GetLeftIndex(i)];
        private Node GetRightChild(int i) => nodes[GetRightIndex(i)];
        private Node GetParent(int i) => nodes[GetParentIndex(i)];

        private void Swap(int i1, int i2)
        {
            Node temp = nodes[i1];
            nodes[i1] = nodes[i2];
            nodes[i2] = temp;
        }

        public bool IsEmpty() => size == 0;

        private void RecalculateDown()
        {
            int index = 0;
            while(HasLeftChild(index))
            {
                int left = GetLeftIndex(index);
                if (HasRightChild(index) && GetRightChild(index) < GetLeftChild(index))
                    left = GetRightIndex(index);

                if (nodes[left] >= nodes[index])
                    break;

                Swap(left, index);
                index = left;
            }
        }

        private void RecalculateUp()
        {
            int index = size - 1;
            while(!IsRoot(index) && nodes[index] < GetParent(index))
            {
                int pI = GetParentIndex(index);
                Swap(pI, index);
                index = pI;
            }
        }
    }

    // helper class: keeps track of scores, position, and pathing
    private class Node
    {
        public Coordinate pos;
        public Node from;
        public float gScore;
        public float fScore;
        public Node(Coordinate p, Node f)
        {
            pos = p;
            from = f;
        }
        public void setScore(float newG, Coordinate end)
        {
            gScore = newG;
            // tentative distance
            int qp = pos.x - (pos.z + (pos.z & 1)) / 2;
            int rp = pos.z;

            int qe = end.x - (end.z + (end.z & 1)) / 2;
            int re = end.z;

            int qdiff = qe - qp;
            int rdiff = re - rp;

            float hScore = (((Mathf.Abs(qdiff) + Mathf.Abs(qdiff + rdiff) + Mathf.Abs(rdiff)) / 2f));// + (Mathf.Abs(pos.x - end.x) + Mathf.Abs(pos.z - end.z)))/2f;
            fScore = gScore + hScore * 1.5f; // magic number to make slightly greedier
        }
        public override bool Equals(object obj)
        {
            Node other = (Node)obj;
            return pos == other.pos && from == other.from;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode();
        }

        public static bool operator>(Node n1, Node n2) => n1.fScore > n2.fScore;
        public static bool operator<(Node n1, Node n2) => n1.fScore < n2.fScore;
        public static bool operator>=(Node n1, Node n2) => n1.fScore >= n2.fScore;
        public static bool operator<=(Node n1, Node n2) => n1.fScore <= n2.fScore;
    }
}
