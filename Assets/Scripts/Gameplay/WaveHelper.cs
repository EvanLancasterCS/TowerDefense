using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveHelper
{
    [SerializeField] private SpawnInformation[] spawns;
    [SerializeField] private float disbursementTime;
    int index = 0;

    public WaveHelper(SpawnInformation[] _spawns, float _disbursementTime)
    {
        spawns = _spawns;
        disbursementTime = _disbursementTime;
        index = 0;
    }

    public float GetDisbursementTime() => disbursementTime;

    public void ResetWave() { index = 0; }

    public SpawnInformation GetNextSpawnInfo()
    {
        if (index < spawns.Length)
            return spawns[index++];
        return null;
    }

    public List<int> UsedDirs()
    {
        List<int> used = new List<int>();
        for(int i = 0; i < spawns.Length; i++)
            if (!used.Contains(spawns[i].GetSpawnIndex()))
                used.Add(spawns[i].GetSpawnIndex());
        return used;
    }

    [System.Serializable]
    public class SpawnInformation
    {
        [SerializeField] private int enemyID;
        [SerializeField] private int numSpawns;
        [SerializeField] private dirs spawnIndex;
        public enum dirs { West, South, North, East };

        public SpawnInformation(int id, int spawns, dirs index)
        {
            enemyID = id;
            numSpawns = spawns;
            spawnIndex = index;
        }

        public int GetID() => enemyID;
        public int GetNumSpawns() => numSpawns;
        public int GetSpawnIndex() => (int)spawnIndex;
    }
}
