using UnityEngine;

public class GameSelector : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam;

    //public ISelectable selectedObject;
    //public ISelectable[] selectedObjectList;

    public string currentlySelectedObject = "";
    [SerializeField] private LayerMask layerMask;

    public delegate void ActionClick();
    public static event ActionClick onClick;
    
    bool dragSelect;
    Vector3 pointOne;
    Vector3 pointTwo;

    void Start(){
        dragSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            pointOne = Input.mousePosition;
        }
        
        if(Input.GetMouseButton(0)){
            if((pointOne - Input.mousePosition).magnitude > 40)
            {
                dragSelect = true;
            }
        }

        if(Input.GetMouseButtonUp(0)){
            if(dragSelect == false){                
                Select();
            }
        }




    }


    void Select(){        
        /*
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(fpsCam.ScreenPointToRay(Input.mousePosition));
        if(rayHit.collider!=null){
            GameObject obj = rayHit.collider.GetComponent<GameObject>();
            if(obj != null){
                if(selectedObject!=null)
                    selectedObject.isSelected = false;
                selectedObject = obj;
                currentlySelectedObject = obj.name;
                selectedObject.isSelected = true;
            }else{
                if(selectedObject!=null)
                    selectedObject.isSelected = false;
                selectedObject = null;
                currentlySelectedObject = "";
                Debug.Log("not Selectable");
            }
        }else{
            selectedObject = null;
            currentlySelectedObject = "";
        } 
        */
    }

    public void ButtonClick(){
        if(onClick != null){
            onClick();
        }
    }
}
