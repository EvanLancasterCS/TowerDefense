using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker inst;
    public static int mainScore = 0;
    public int tempScore;
    public bool updatingScore = false;

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
        tempScore = mainScore;
        mainScore += val;
        if(!updatingScore)
            StartCoroutine(SceneSwitch());
    }

    public int getScore()
    {
        return mainScore;
    }

    IEnumerator SceneSwitch()
    {
        updatingScore = true;

        while(tempScore < mainScore)
        {
            tempScore++;
            yield return new WaitForSeconds(0.01f);
            UIManager.instance.SetPlayerScore(tempScore);
        }

        updatingScore = false;
    }

}
