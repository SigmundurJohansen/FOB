using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Threading;
using UnityEngine.Rendering;
using TMPro;
using SF = UnityEngine.SerializeField;

public class GameController : MonoBehaviour
{
    private static GameController m_Instance;
    public static GameController Instance { get { return m_Instance; } }

    public GameObject listViewPrefab;
    public GameObject listViewParent;
    
    private List<string> gameUnitList = new List<string>();
    private List<GameObject> unitListView = new List<GameObject>();
    private int ID = 0;

    public delegate void UpdateListViewHandler();
    public event UpdateListViewHandler ViewUpdated;

    void Awake()
    {
        m_Instance = this;
        m_Instance.ViewUpdated += OnGui;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }
    public int GetUnitListLength()
    {
        return gameUnitList.Count;
    }

    public int AddID(){ID++; return ID;}

    public int GetID(){return ID;}

    public void AddUnitName(string _name)
    {
        gameUnitList.Add(_name);
        ECSController.Instance.CreateEntities(1);
    }
    public void RemoveUnitName()
    {
        if(gameUnitList.Count>=1)
            gameUnitList.RemoveAt(gameUnitList.Count-1);
    }
    public void SetUnitName(int _index, string _name)
    {
        gameUnitList[_index] = _name;
    }

    void Update()
    {
        if(unitListView.Count != gameUnitList.Count)
        {
            ViewUpdated.Invoke();
        }
    }

    void OnGui()
    {
        foreach(var eView in unitListView)
        {
            Destroy(eView);
        }
        foreach(var munit in gameUnitList)
        {
            GameObject newObject = Instantiate(listViewPrefab) as GameObject;
            newObject.SetActive(true);
            GameObject newNameObject = listViewPrefab.transform.GetChild(0).gameObject;
            TextMeshProUGUI newTextObject = newNameObject.GetComponent<TextMeshProUGUI>();
            newTextObject.text = munit;
            newObject.transform.SetParent(listViewParent.transform.parent, true);
            unitListView.Add(newObject);
        }
    }

}

public class GameUnit
{
    int id;
    string name;
}