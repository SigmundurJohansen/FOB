using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class UIController : MonoBehaviour
{
    [SF] public GameObject entityListView;
    [SF] public GameObject buildingListView;
    private bool entityIsActive = false;
    private bool buildingIsActive = false;
    [SF] public GameObject map;
    private bool mapIsActive = true;
    public GameObject menuPanel;

    void Awake()
    {
        entityListView.SetActive(false);
        buildingListView.SetActive(false);
        map.SetActive(true);
    }
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenu();
        }
        if(Input.GetKeyDown("u"))
        {
            //rectTransform.Set
            if(entityIsActive)
            {
                entityListView.SetActive(false);
                entityIsActive = false;
            }else
            {
                entityListView.SetActive(true);
                entityIsActive = true;
                entityListView.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }
        if(Input.GetKeyDown("b"))
        {
            //rectTransform.Set
            if(buildingIsActive)
            {
                buildingListView.SetActive(false);
                buildingIsActive = false;
            }else
            {
                buildingListView.SetActive(true);
                buildingIsActive = true;
                buildingListView.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }
        if(Input.GetKeyDown("m"))
        {
            //rectTransform.Set
            if(mapIsActive)
            {
                map.SetActive(false);
                mapIsActive = false;
            }else
            {
                map.SetActive(true);
                mapIsActive = true;
                map.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }
    }

    public void UnitMenu()
    {
        if(entityIsActive)
        {
            entityListView.SetActive(false);
            entityIsActive = false;
        }else
        {
            entityListView.SetActive(true);
            entityIsActive = true;
            entityListView.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    public void BuildingMenu()
    {
        if(buildingIsActive)
        {
            buildingListView.SetActive(false);
            buildingIsActive = false;
        }else
        {
            buildingListView.SetActive(true);
            buildingIsActive = true;
            buildingListView.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    public void MiniMap()
    {
        if(mapIsActive)
        {
            map.SetActive(false);
            mapIsActive = false;
        }else
        {
            map.SetActive(true);
            mapIsActive = true;
            map.GetComponent<RectTransform>().SetAsLastSibling();
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

    public void BuildBuilding()
    {
        Debug.Log("buildingigngign!!!");
    }
}
