using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class Menu : MonoBehaviour
{
    

    public void StartGame()
    {
        ChangeScene("GameScene");
    }
    public void WorldGeneration()
    {
        ChangeScene("WorldGeneration");
    }
    public void MainMenu()
    {
        ChangeScene("Start");
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit ();
        //SceneManager.LoadScene("Blank");
    }

    public void ChangeScene(string _scene)
    {
        if(World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            var systems = World.DefaultGameObjectInjectionWorld.Systems;
            foreach(var s in systems)
            {
                s.Enabled = false;
            }
            World.DefaultGameObjectInjectionWorld.Dispose();
            DefaultWorldInitialization.Initialize("Default World", false);
        }
        SceneManager.LoadScene(_scene);
    }
}