using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Timers;
using SF = UnityEngine.SerializeField;

public class GameSelector : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;
    public GameObject selectedObject;
    public Transform selectionAreaTransform;
    public static GameSelector myGameSelector;
    public string currentlySelectedObject = "";
    [SF] public LayerMask layerMask;
    [SF] public RectTransform selectSquareImage;
    [SF] public RectTransform buildingSquareImage;
    bool dragSelect = false;
    private bool Drag = false;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 Origin;
    private Vector3 Diference;
    Vector2 pointOne;
    Vector2 pointTwo;
    public CameraController myCameraController;
    private readonly Timer _MouseSingleClickTimer = new Timer();

    void Start()
    {
        selectSquareImage.gameObject.SetActive(false);
        myGameSelector = this;
        _MouseSingleClickTimer.Interval = 300;
        _MouseSingleClickTimer.Elapsed += SingleClick;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            dragSelect = false;
            selectSquareImage.gameObject.SetActive(false);
            buildingSquareImage.gameObject.SetActive(false);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointOne = WorldPosition();
                startPos = Input.mousePosition;
                if (GameController.Instance.GetSelectionSate() == 1)
                {
                    GameController.Instance.SetSelectionState(0);
                }
            }
            if (Input.GetMouseButton(0))
            {
                pointTwo = WorldPosition();
                endPos = Input.mousePosition;
                if ((pointOne - pointTwo).magnitude > 0.2f)
                {
                    if (GameController.Instance.GetSelectionSate() == 0)
                        MakeSelectionBox();
                    else
                        MakeBuildingBox();
                    dragSelect = true;
                }
                else
                {
                    dragSelect = false;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (dragSelect == false)
                {
                    //Select();
                }
                else
                {
                }
                selectSquareImage.gameObject.SetActive(false);
                if (_MouseSingleClickTimer.Enabled == false)
                {
                    // ... timer start
                    _MouseSingleClickTimer.Start();
                    // ... wait for double click...
                    return;
                }
                else
                {
                    //Doubleclick performed - Cancel single click
                    _MouseSingleClickTimer.Stop();
                    myCameraController.SetCameraPosition(Input.mousePosition);
                    //Do your stuff here for double click...
                }
            }

            if (Input.GetMouseButton(2))
            {
                Diference = (fpsCam.ScreenToWorldPoint(Input.mousePosition)) - fpsCam.transform.position;
                if (Drag == false)
                {
                    Drag = true;
                    Origin = fpsCam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                Drag = false;
            }
            if (Drag == true)
            {
                fpsCam.transform.position = Origin - Diference;
            }
        }
    }

    void SingleClick(object _o, System.EventArgs _e)
    {
        _MouseSingleClickTimer.Stop();

        //Do your stuff for single click here....
    }

    void MakeSelectionBox()
    {
        if (!selectSquareImage.gameObject.activeInHierarchy)
        {
            selectSquareImage.gameObject.SetActive(true);
        }
        Vector3 centre = (startPos + (endPos)) / 2f;
        selectSquareImage.position = centre;

        //Change the size of the square
        float sizeX = Mathf.Abs(startPos.x - endPos.x);
        float sizeY = Mathf.Abs(startPos.y - (endPos.y));
        selectSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
    }

    void MakeBuildingBox()
    {
        if (!buildingSquareImage.gameObject.activeInHierarchy)
        {
            buildingSquareImage.gameObject.SetActive(true);
        }
        Vector3 centre = (startPos + (endPos)) / 2f;
        buildingSquareImage.position = centre;

        //Change the size of the square
        float sizeX = Mathf.Abs(startPos.x - endPos.x);
        float sizeY = Mathf.Abs(startPos.y - (endPos.y));
        buildingSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
    }
    void Deselect()
    {
        if (selectedObject != null)
        {
            selectionAreaTransform.gameObject.SetActive(false);
            selectedObject.GetComponent<SpriteRenderer>().color = Color.white;
            currentlySelectedObject = "";
        }
        selectedObject = null;
    }

    void Select()
    {
        Vector3 mousePos = fpsCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        RaycastHit2D rayHit = Physics2D.Raycast(mousePos, fpsCam.transform.position, 10f, layerMask);

        if (rayHit.collider != null)
        {
            Debug.Log(rayHit.collider.name);
            GameObject obj = rayHit.collider.gameObject;
            if (obj != null)
            {
                if (selectedObject != null)
                    Deselect();
                selectedObject = obj;
                currentlySelectedObject = obj.name;
                selectionAreaTransform.gameObject.SetActive(true);
                selectionAreaTransform.gameObject.transform.position = pointOne;
                selectedObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                Deselect();
                Debug.Log("not Selectable");
            }
        }
        else
        {
            Deselect();
        }
    }

    public Vector3 WorldPosition()
    {
        Vector3 mousePos = fpsCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        return mousePos;
    }
}
