using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minZ = -10f;
    public float maxZ = -100f;

    public GameObject player; 
    private Vector3 offset;
     void Start () 
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }
    void Update()
    {
        
        Vector3 pos = transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z += scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.x = player.transform.position.x + offset.x;
        pos.y = player.transform.position.y + offset.y;
        pos.z = Mathf.Clamp(pos.z, - maxZ, - minZ);
        transform.position = pos;
    }

    public void SetStaticCamera(){
        Vector3 pos = transform.position;

        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.y += panSpeed * Time.deltaTime;
        if(Input.GetKey("s") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.y -= panSpeed * Time.deltaTime;
        if(Input.GetKey("a") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.x -= panSpeed * Time.deltaTime;
        if(Input.GetKey("d") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos.x += panSpeed * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z += scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, - panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, - panLimit.y, panLimit.y);
        pos.z = Mathf.Clamp(pos.z, - maxZ, - minZ);

        transform.position = pos;

    }
}
