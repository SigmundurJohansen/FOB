using UnityEngine;

public class GameSelector : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    [SerializeField] private LayerMask layerMask;

    public delegate void ActionClick();
    public static event ActionClick onClick;
    
    public void ButtonClick(){
        if(onClick != null){
            onClick();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetMouseButtonDown(0))
        {
            Selector3();
        }
        
    }

    void SelectTarget()
    {
        RaycastHit hit;

        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range,layerMask)){
            Debug.Log("Select :" + hit.transform.name);
            Unit unit = hit.transform.GetComponent<Unit>();
            if(unit != null){
                unit.TakeDamage(50f);
            }
        }
        else
        {
            Debug.Log("Did not Hit");
        }
    }

    void anotherSelector(){
        if(Input.GetMouseButtonDown(0)){
            Ray rayOrigin = fpsCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            if(Physics.Raycast(rayOrigin, out hitInfo)){
                ISelectable obj = hitInfo.collider.GetComponent<ISelectable>();
                if(obj != null){
                    Debug.Log("selecting: " + obj.Name());
                    obj.Damage(10);
                }else{
                    Debug.Log("selected nothing");
                }
            }
        }
    }

    void Selector3(){
        
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(fpsCam.ScreenPointToRay(Input.mousePosition));
        if(rayHit.collider!=null){
            ISelectable obj = rayHit.collider.GetComponent<ISelectable>();
            if(obj != null){
                Debug.Log("selecting: " + obj.Name());
                obj.Damage(10);
            }else{
                Debug.Log("nonSelectable");
            }
        }else{
            Debug.Log("nothing selected");
        }
    }
}
