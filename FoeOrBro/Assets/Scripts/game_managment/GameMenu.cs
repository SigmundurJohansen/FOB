using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameMenu : MonoBehaviour
{
    
    public Button firstButton, secondButton, thirdButton, fourthButton, fifthButton;
    // Start is called before the first frame update
    void Start()
    {
        firstButton.onClick.AddListener(MainMenu);
        secondButton.onClick.AddListener(QuitGame);        
    }

    public void MainMenu()
    {
        //SceneManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        //SceneManager.LoadScene("Blank");
    }
}
