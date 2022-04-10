using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyHelper
{
    [SerializeField] private string name; // difficulty name
    [SerializeField] private Color color; // difficulty color
    [SerializeField] private WaveHelper[] waveChoices; // set of waves to choose from
    [SerializeField] private float difficultyIncreasePerWave; // amt to increase difficulty by after defeating wave
    [SerializeField] private float baseHealthMultiplier = 1f, healthMultiplierPerDifficulty;
    [SerializeField] private float baseSpeedMultiplier = 1f, speedMultiplierPerDifficulty;
    [SerializeField] private float difficultyThreshold; // difficulty value to reach for this difficulty to trigger
    [SerializeField] private bool linear = false; // choose wave choices in order instead of random
    [SerializeField] private int numTowersGiven; // preferably maxmimum is 5, but we should try to keep it low
    [SerializeField] private int[] towerProbabilities; // set of probabilities to set towerpresets
    private System.Random rand;
    private int linearIndex = 0;

    public DifficultyHelper(string _name, WaveHelper[] _waveChoices, float _difficultyThreshold, bool _linear, int _numTowersGiven)
    {
        name = _name;
        waveChoices = _waveChoices;
        difficultyThreshold = _difficultyThreshold;
        linear = _linear;
        numTowersGiven = _numTowersGiven;
    }

    public WaveHelper GetWave()
    {
        if(rand == null)
            rand = DifficultyInfo.instance.difficultyRand;

        if (linear)
        {
            if (linearIndex >= waveChoices.Length)
                linearIndex = 0;
            return waveChoices[linearIndex++];
        }

        int index = rand.Next(0, waveChoices.Length);
        return waveChoices[index];
    }

    public string GetName() => name;
    public Color GetColor() => color;
    public float GetDifficultyIncrease() => difficultyIncreasePerWave;

    public float GetModifiedHealth(float health, float currDifficulty) => health * baseHealthMultiplier + (health * (currDifficulty - difficultyThreshold) * healthMultiplierPerDifficulty);
    public float GetModifiedSpeed(float speed, float currDifficulty) => speed * baseSpeedMultiplier + (speed * (currDifficulty - difficultyThreshold) * speedMultiplierPerDifficulty);

    public float GetThreshold()
    {
        return difficultyThreshold;
    }

    public int GetNumTowersGiven()
    {
        return numTowersGiven;
    }

    public void SetWorldTowerProbabilities()
    {
        TowerPresets.SetProbabilities(towerProbabilities);
    }
}
