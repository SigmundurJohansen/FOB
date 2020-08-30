using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void WorldGeneration()
    {
        SceneManager.LoadScene("WorldGeneration");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Start");
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit ();
        //SceneManager.LoadScene("Blank");
    }
}