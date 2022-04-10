using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyInfo : MonoBehaviour
{
    public static DifficultyInfo instance;

    public DifficultyHelper[] difficulties;
    private DifficultyHelper currentDifficulty;
    private int currentDifficultyIndex = -1;


    private void Start()
    {
        instance = this;
    }
    
    // returns true if difficulty changed
    public bool UpdateDifficulty(float difficulty)
    {
        if(currentDifficultyIndex == -1)
        {
            currentDifficulty = difficulties[0];
            currentDifficultyIndex = 0;
            return true;
        }
        else
        {
            int nextIndex = currentDifficultyIndex + 1;
            if (nextIndex < difficulties.Length)
            {
                if(difficulty > difficulties[nextIndex].GetThreshold())
                {
                    currentDifficulty = difficulties[nextIndex];
                    currentDifficultyIndex = nextIndex;
                    return true;
                }
            }
        }
        return false;
    }

    public WaveHelper GetWave()
    {
        return currentDifficulty.GetWave();
    }

    public DifficultyHelper GetDifficulty() => currentDifficulty;
}
