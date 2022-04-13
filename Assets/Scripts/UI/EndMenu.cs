using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerScoreValText;
    // Start is called before the first frame update

    void Start()
    {
        playerScoreValText.text = ScoreTracker.mainScore.ToString();
    }

    public void restartGame()
    {
        StartCoroutine(SceneSwitch());
    }

    IEnumerator SceneSwitch()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        yield return null;
        SceneManager.UnloadSceneAsync("EndScene");
    }
}
