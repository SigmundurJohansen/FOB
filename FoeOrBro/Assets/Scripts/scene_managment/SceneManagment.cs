using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagment : MonoBehaviour
{

    public Button firstButton, secondButton, thirdButton, fourthButton;

    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        firstButton.onClick.AddListener(StartGame);
        secondButton.onClick.AddListener(WorldGeneration);
        thirdButton.onClick.AddListener(MainMenu);
        fourthButton.onClick.AddListener(QuitGame);

    }


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
