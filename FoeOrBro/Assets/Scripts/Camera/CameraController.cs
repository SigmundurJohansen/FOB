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
    public float minZ = -1f;
    public float maxZ = -10f;
    public GameObject player; 
    public Camera whoAmI;
    float mDelta = 10f; // Pixels. The width border at the edge in which the movement work
    float mSpeed = 3.0f; // Scale. Speed of the movement

    void Start () 
    {
    }
    void Update()
    {
        #region camera borders
        if ( Input.mousePosition.x >= Screen.width - mDelta )
            transform.position += transform.right * Time.deltaTime * mSpeed;
        if ( Input.mousePosition.x <= 0 + mDelta )
            transform.position -= transform.right * Time.deltaTime * mSpeed;
        if ( Input.mousePosition.y >= Screen.height - mDelta )
            transform.position += transform.up * Time.deltaTime * mSpeed;
        if ( Input.mousePosition.y <= 0 + mDelta )
            transform.position -= transform.up * Time.deltaTime * mSpeed;
        #endregion
        
        Vector3 pos = transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize  -= scroll * scrollSpeed * 100f * Time.deltaTime;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZ,  maxZ);
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

    public void SetCameraFollow(GameObject _target, Transform _pos){
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        Vector3 offset = transform.position - player.transform.position;

        Vector3 pos = _pos.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize  -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.x = _target.transform.position.x + offset.x;
        pos.y = _target.transform.position.y + offset.y;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZ,  maxZ);
        transform.position = pos;
    }

    public void SetCameraPosition(Vector3 _pos){
        Vector3 tempPos = WorldPosition();
        transform.position = tempPos;
    }

    public Vector3 WorldPosition(){        
        Vector3 mousePos = whoAmI.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mousePos.z = transform.position.z;
        return mousePos;
    }
}
