using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldUIManager : MonoBehaviour
{

    public Toggle heightToggle;
    public Toggle heatToggle;
    public Toggle moistureToggle;
    public Toggle biomeToggle;

    public GameObject heightPanel;
    public GameObject heatPanel;
    public GameObject moisturePanel;
    public GameObject riverPanel;
    public GameObject menuPanel;
    

    bool heightBool = true;
    public void ToggleHeight()
    {
        if(heightBool)
            heightBool = false;
        else
            heightBool = true;
        heightPanel.SetActive(heightBool);
    }
    bool heatBool = true;
    public void ToggleHeat()
    {
        if(heatBool)
            heatBool = false;
        else
            heatBool = true;
        heatPanel.SetActive(heatBool);
    }

    bool moistureBool = true;
    public void ToggleMoisture()
    {
        if(moistureBool)
            moistureBool = false;
        else
            moistureBool = true;
        moisturePanel.SetActive(moistureBool);
    }

    bool riverBool = true;
    public void ToggleRiver()
    {
        if(riverBool)
            riverBool = false;
        else
            riverBool = true;
        riverPanel.SetActive(riverBool);
    }

    public void Save()
    {
        Generator.Instance.SaveMap();
    }

    public void Load()
    {
        Generator.Instance.LoadMap();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
    }
    
    bool menuBool = false;
    public void ShowMenu()
    {
        if(menuBool)
            menuBool = false;
        else
            menuBool = true;
        menuPanel.SetActive(menuBool);
    }

}
