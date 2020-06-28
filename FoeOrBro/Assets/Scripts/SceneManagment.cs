using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Start");
    }
    public void QuitGame()
    {
        SceneManager.LoadScene("Blank");
    }
    public void CaveGenerator()
    {
        SceneManager.LoadScene("CaveGenerator");
    }
}
