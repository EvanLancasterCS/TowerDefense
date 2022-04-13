using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker inst;
    public static int mainScore = 0;

    private void Awake()
    {
        inst = this;
        DontDestroyOnLoad(transform.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    public void addToScore(int val)
    {
        mainScore += val;
    }

    public int getScore()
    {
        return mainScore;
    }
}
