using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagment : MonoBehaviour
{

    public Button firstButton, secondButton, thirdButton;

    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        firstButton.onClick.AddListener(StartGame);
        secondButton.onClick.AddListener(MainMenu);
        thirdButton.onClick.AddListener(QuitGame);

    }


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
