using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuButtons : MonoBehaviour
{
    public void BeginNewGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneBuildIndex = currentScene.buildIndex;
        SceneManager.LoadScene(sceneBuildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
