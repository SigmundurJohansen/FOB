using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public void BuildBuildingHouse()
    {
        Debug.Log("house!!!");
        Debug.Log("set state to 0");
        GameController.Instance.SetSelectionState(0);
    }
    public void BuildBuildingFarm()
    {
        Debug.Log("farm!!!");
        Debug.Log("Set state to 1");
        GameController.Instance.SetSelectionState(1);
    }
    public void BuildBuildingAltar()
    {
        Debug.Log("altar!!!");
    }
    public void BuildBuildingSmithy()
    {
        Debug.Log("smithy!!!");
        ECSController.instance.CreateEntityBuilding("smithy", true);
        GameController.Instance.SetSelectionState(1);
    }
    public void BuildBuildingLumberMill()
    {
        Debug.Log("lumbermill!!!");
    }
    public void BuildBuildingSilo()
    {
        Debug.Log("silo!!!");
    }
    public void BuildBuildingWarehouse()
    {
        Debug.Log("warehouse!!!");
    }
}
