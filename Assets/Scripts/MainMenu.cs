using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        StartCoroutine(SceneSwitch());
        
        
    }

    public void showHelp()
    {

    }

    public void showMenu()
    {

    }

    IEnumerator SceneSwitch()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        yield return null;
        SceneManager.UnloadSceneAsync("MenuScene");
    }
}
