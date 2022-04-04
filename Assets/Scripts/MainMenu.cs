using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject gui_Menu;
    public GameObject gui_Help;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void startGame()
    {
        StartCoroutine(SceneSwitch());
        
        
    }

    public void showHelp()
    {
        gui_Menu.SetActive(false);
        gui_Help.SetActive(true);

    }

    public void showMenu()
    {
        gui_Menu.SetActive(true);
        gui_Help.SetActive(false);
    }

    IEnumerator SceneSwitch()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        yield return null;
        SceneManager.UnloadSceneAsync("MenuScene");
    }
}
