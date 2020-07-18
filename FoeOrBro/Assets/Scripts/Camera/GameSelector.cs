
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class GameSelector : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;
    //public ISelectable selectedObject;
    //public ISelectable[] selectedObjectList;
    public GameObject selectedObject;
    public Transform selectionAreaTransform;


    public string currentlySelectedObject = "";

    [SerializeField]
    public LayerMask layerMask;

    [SerializeField]
    public RectTransform selectSquareImage;

    bool dragSelect = false;
    Vector3 startPos;
    Vector3 endPos;


    Vector2 pointOne;
    Vector2 pointTwo;

    void Start(){        
        selectSquareImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {          
            pointOne = WorldPosition();
            startPos = Input.mousePosition;
        }    
        if(Input.GetMouseButton(0)){
            Vector2 pointNow = WorldPosition();    
            endPos = Input.mousePosition; 
            if((pointOne - pointNow).magnitude > 1)
            {
                MakeSelectionBox();
                dragSelect = true;
            }else{
                dragSelect = false;
            }

        }
        if(Input.GetMouseButtonUp(0)){
            selectSquareImage.gameObject.SetActive(false);
            if(dragSelect == false){                
                Select();
            }else{
            }
        }
    }

    void MakeSelectionBox(){
                if(!selectSquareImage.gameObject.activeInHierarchy)
                {
                    selectSquareImage.gameObject.SetActive(true);
                }
                //startPos = new Vector3(pointOne.x ,pointOne.y, 0);
                //endPos = new Vector3(pointNow.x ,pointNow.y, 0);
                Vector3 centre = (startPos + endPos) / 2f;
                selectSquareImage.position = centre;

                //Change the size of the square
                float sizeX = Mathf.Abs(startPos.x - endPos.x);
                float sizeY = Mathf.Abs(startPos.y - endPos.y);

                selectSquareImage.sizeDelta = new Vector2(sizeX,sizeY);

    }

    void Deselect(){
        if(selectedObject != null){
            selectionAreaTransform.gameObject.SetActive(false);
            selectedObject.GetComponent<SpriteRenderer>().color = Color.white;
            currentlySelectedObject = "";
        }
        selectedObject = null;
    }

    void Select(){        
        Vector3 mousePos = fpsCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));        
        RaycastHit2D rayHit = Physics2D.Raycast(mousePos, fpsCam.transform.position, 10f,layerMask);

        if(rayHit.collider!=null){
            Debug.Log(rayHit.collider.name);
            GameObject obj = rayHit.collider.gameObject;
            if(obj != null){
                if(selectedObject!=null)
                    Deselect();
                selectedObject = obj;
                currentlySelectedObject = obj.name;
                selectionAreaTransform.gameObject.SetActive(true);
                selectionAreaTransform.gameObject.transform.position = pointOne;
                selectedObject.GetComponent<SpriteRenderer>().color = Color.green;
            }else{
                Deselect();
                Debug.Log("not Selectable");
            }
        }else{
            Deselect();
        } 
    }
    

    public Vector3 WorldPosition(){
        
        Vector3 mousePos = fpsCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        return mousePos;
    }
}
