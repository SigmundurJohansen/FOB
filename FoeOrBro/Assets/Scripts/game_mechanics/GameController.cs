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
    public Camera myCamera;

    public GameObject listViewPrefab;
    public GameObject listViewParent;

    public GameObject healthBarPrefab;
    public GameObject healthBarParent;
    
    private List<GameUnit> gameUnitList = new List<GameUnit>();
    private List<GameObject> unitListView = new List<GameObject>();
    private int ID = 0;

    public delegate void UpdateListViewHandler();
    public event UpdateListViewHandler ViewUpdated;

    private Vector3 startPosition = new Vector3(-100,-100, -1);
    void Awake()
    {
        m_Instance = this;
        m_Instance.ViewUpdated += OnGui;
    }

    void OnDestroy(){m_Instance = null;}

    public int GetUnitListLength()
    {
        return gameUnitList.Count;
    }
    public void SetPosition(int _id, float _x, float _y)
    {
        if(_id>GetID())
        {
            Debug.Log(_id);
            return;
        }
        if( gameUnitList[_id]!=null)
            gameUnitList[_id].SetPosition(new Vector3(_x,_y,-0.1f));
    }

    public int AddID(){ID++; return ID;}

    public int GetID(){return ID;}

    public void AddUnit(string _name, Vector3 _position)
    {
        GameUnit newUnit = new GameUnit(GetID(),_name,_position);
        gameUnitList.Add(newUnit);
   
        var krec = healthBarParent.GetComponent<RectTransform>();
        GameObject healthBar = Instantiate(healthBarPrefab) as GameObject;
        healthBar.transform.SetParent(krec, true);//.transform.parent
        Vector3 position = WorldPosition(_position);
        Vector2 anchor = _position / krec.localScale.x;
        position= position / krec.localScale.x;
        position = position - new Vector3(512, 384,0);
        healthBar.GetComponent<RectTransform >().anchoredPosition = position;
        healthBar.SetActive(true);
        newUnit.menu = healthBar;
        AddID();
    }
    public void RemoveUnitName()
    {
        if(gameUnitList.Count>=1)
            gameUnitList.RemoveAt(gameUnitList.Count-1);
    }

    public void SetUnitName(int _index, string _name)
    {
        gameUnitList[_index].SetName(_name);
    }

    void Update()
    {
        foreach(var unit in gameUnitList){
            Vector3 position = WorldPosition(unit.GetPosition());
            position = position - new Vector3(512, 360,0);
            unit.menu.GetComponent<RectTransform >().anchoredPosition = position;
        }
    }
    


    public void OnGui()
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
            newTextObject.text = munit.GetName();
            newObject.transform.SetParent(listViewParent.transform.parent, true);
            unitListView.Add(newObject);
        }
    }

    public Vector3 ScreenPosition(Vector3 _pos ){        
        Vector3 screenPos = myCamera.ScreenToWorldPoint(new Vector3(_pos.x, _pos.y, 0));
        return screenPos;
    }
    public Vector3 WorldPosition(Vector3 _pos ){
        Vector3 worldPos = myCamera.WorldToScreenPoint(new Vector3(_pos.x, _pos.y, 0));
        return worldPos;
    }
}

public class GameUnit
{
    int id;
    string name;
    Vector3 position;
    public GameObject menu;
    public GameUnit(int _id,string _name,Vector3 _pos){
        id = _id;
        name = _name;
        position = _pos;
    }
    public void SetName(string _name){name = _name;}
    public string GetName(){return name;}
    public void SetPosition(Vector3 _pos){position = _pos;}
    public Vector3 GetPosition(){return position;}
}