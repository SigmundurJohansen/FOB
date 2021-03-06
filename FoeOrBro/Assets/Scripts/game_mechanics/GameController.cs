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
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class GameController : MonoBehaviour
{
    private static GameController m_Instance;
    public static GameController Instance { get { return m_Instance; } }
    public static float GameTimeScale { get; set; }
    public bool attackState = false;
    public int selectionState;
    public Camera myCamera;

    public GameObject listViewPrefab;
    public GameObject listViewParent;

    
    public GameObject listBuildingPrefab;
    public GameObject listBuildingParent;

    public GameObject healthBarPrefab;
    public GameObject healthBarParent;

    private List<GameUnit> gameUnitList = new List<GameUnit>();
    private List<GameObject> unitListView = new List<GameObject>();
    private int ID = 0;
    private int count = 0;

    public delegate void UpdateListViewHandler();
    //public event UpdateListViewHandler ViewUpdated;
    private float fixedDeltaTime = 0;

    private Vector3 startPosition = new Vector3(-100, -100, -1);
    void Awake()
    {
        m_Instance = this;
        selectionState = 0;
        GameTimeScale = 1;
        //m_Instance.ViewUpdated += OnGui;
    }

    void OnDestroy() { m_Instance = null; }

    void Update()
    {
        foreach (var unit in gameUnitList)
        {
            Vector3 position = WorldPosition(unit.GetPosition());
            float size = CameraController.Instance.GetSize();
            size = Mathf.Clamp(6 - 1.4f * size, 1, 6);
            //unit.menu.GetComponent<RectTransform>().localScale = new Vector3(size, size, size);

            //unit.menu.GetComponent<Slider>().value = unit.health;

            position = position - new Vector3(Screen.width / 2, Screen.height / 2 - size * 15, 0);
            //unit.menu.GetComponent<RectTransform>().anchoredPosition = position;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetAttackState(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
            // Adjust fixed delta time according to timescale
            // The fixed delta time will now be 0.02 frames per real-time second
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }
    }

    public void SetSelectionState(int _state)
    {
        selectionState = _state;
    }
    public int GetSelectionSate()
    {
        return selectionState;
    }
    public void SetAttackState(bool _state)
    {
        attackState = _state;
    }
    public bool GetAttackState()
    {
        return attackState;
    }


    public int GetUnitListLength()
    {
        return gameUnitList.Count;
    }
    public void SetPosition(int _id, float _x, float _y)
    {
        for (int i = 0; i < gameUnitList.Count; i++)
        {
            if (gameUnitList[i].GetID() == _id)
            {
                gameUnitList[i].position = new Vector3(_x, _y, -0.1f);
            }
        }
    }

    public int AddID() { ID++; count++; return ID; }

    public int GetID() { return ID; }

    public void AddUnit(string _name, Vector3 _position, float _health)
    {
        GameUnit newUnit = new GameUnit(ID, _name, _position, _health);
        gameUnitList.Add(newUnit);

        /*
        var krec = healthBarParent.GetComponent<RectTransform>();
        GameObject healthBar = Instantiate(healthBarPrefab) as GameObject;
        healthBar.transform.SetParent(krec, true);//.transform.parent
        Vector3 position = WorldPosition(_position);
        Vector2 anchor = _position / krec.localScale.x;
        position = position / krec.localScale.x;
        position = position - new Vector3(512, 384, 0);
        healthBar.GetComponent<RectTransform>().anchoredPosition = position;
        healthBar.SetActive(true);
        newUnit.menu = healthBar;
        */
        AddID();
        OnGui();
    }

    public float GetUnitHealth(int _id)
    {
        foreach (var unit in gameUnitList)
        {
            if (unit.GetID() == _id)
            {
                return unit.health;
            }
        }
        return -1f;
    }

    public void SetUnitHealth(int _id, float _amount)
    {
        foreach (var unit in gameUnitList)
        {
            if (unit.GetID() == _id)
            {
                unit.health = _amount;
            }
        }
    }

    public void RemoveUnit(int _id)
    {
        int die = -1;
        for (int i = 0; i < gameUnitList.Count; i++)
        {
            if (gameUnitList[i].GetID() == _id)
            {
                die = i;
            }
        }
        if (die > 0)
        {
            Destroy(gameUnitList[die].menu);
            gameUnitList.RemoveAt(die);
        }
        else
            Debug.Log("Error killing unit");
        OnGui();
    }

    public enum damageType
    {
        Physical = 0,
        Fire,
        Ice,
        Magical
    }

    public void DamageUnit(int _id1, int _id2, float _amount, damageType _type)
    {
        string attackerName = "someone";
        foreach (var attacker in gameUnitList)
        {
            if (attacker.GetID() == _id1)
                attackerName = attacker.GetName();
        }
        bool hit = false;
        foreach (var unit in gameUnitList)
        {
            if (unit.GetID() == _id2)
            {
                unit.health = unit.health - _amount;
                Debug.Log(unit.GetName() + " takes " + _amount + " " + _type + " damage from " + attackerName);
                hit = true;
            }
        }
        if (!hit)
            Debug.Log(attackerName + " Attack Missed!");
        OnGui();
    }

    public void SetUnitName(int _index, string _name)
    {
        gameUnitList[_index].SetName(_name);
    }



    public void OnGui()
    {
        foreach (var eView in unitListView)
        {
            Destroy(eView);
        }
        foreach (var munit in gameUnitList)
        {
            GameObject newObject = Instantiate(listViewPrefab) as GameObject;
            newObject.SetActive(true);

            GameObject listName = newObject.transform.GetChild(0).gameObject;
            TextMeshProUGUI newTextObject = listName.GetComponent<TextMeshProUGUI>();
            newTextObject.text = munit.GetName();

            GameObject listHealth = newObject.transform.GetChild(1).gameObject;
            TextMeshProUGUI newTextObject2 = listHealth.GetComponent<TextMeshProUGUI>();
            newTextObject2.text = munit.health.ToString();

            GameObject listNumber = newObject.transform.GetChild(2).gameObject;
            TextMeshProUGUI newTextObject3 = listNumber.GetComponent<TextMeshProUGUI>();
            newTextObject3.text = munit.GetID().ToString();

            newObject.transform.SetParent(listViewParent.transform, true);
            unitListView.Add(newObject);
        }
    }
    public void PopulateBuildingList()
    {
        
    }

    public void SpeedUp()
    {
        if(GameTimeScale > 4)
            GameTimeScale = 4;
        else if (GameTimeScale < 1)
            GameTimeScale = 1;
        else
            GameTimeScale += 1;
        Debug.Log("GameTimeScale: " + GameTimeScale);
        Time.timeScale = 1 * GameTimeScale;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    public void SlowDown()
    {
        if (GameTimeScale >= 2)
            GameTimeScale -= 1;
        else if (GameTimeScale < 2 && GameTimeScale > 1)
            GameTimeScale = 1;
        else if (GameTimeScale <= 1 && GameTimeScale > 0.1f)
            GameTimeScale *= 0.8f;
        Debug.Log("GameTimeScale: " + GameTimeScale);
        Time.timeScale = 1 * GameTimeScale;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    public void PlayPause()
    {
        if (Time.timeScale == 1.0f)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
        // Adjust fixed delta time according to timescale
        // The fixed delta time will now be 0.02 frames per real-time second
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    public Vector3 ScreenPosition(Vector3 _pos)
    {
        Vector3 screenPos = myCamera.ScreenToWorldPoint(new Vector3(_pos.x, _pos.y, 0));
        return screenPos;
    }
    public Vector3 WorldPosition(Vector3 _pos)
    {
        Vector3 worldPos = myCamera.WorldToScreenPoint(new Vector3(_pos.x, _pos.y, 0));
        return worldPos;
    }
}

public class GameUnit
{
    int id;
    string name;
    public float health = 100;
    float maxHealth = 100;
    public Vector3 position;
    public GameObject menu;
    public GameUnit(int _id, string _name, Vector3 _pos, float _health)
    {
        id = _id;
        name = _name;
        position = _pos;
        health = _health;
        maxHealth = _health;
    }
    public int GetID() { return id; }
    public void SetName(string _name) { name = _name; }
    public string GetName() { return name; }
    public void SetPosition(Vector3 _pos) { position = _pos; }
    public Vector3 GetPosition() { return position; }
}