using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneBuildIndex = currentScene.buildIndex;
            SceneManager.LoadScene(sceneBuildIndex + 1);
        }
    }
}
