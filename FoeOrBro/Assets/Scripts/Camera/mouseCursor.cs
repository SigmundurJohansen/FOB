using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour 
{
    private SpriteRenderer myRend;
    public Texture2D handCursor;
    public Texture2D normalCursor;
    public Texture2D targetCursor;
    public Camera myCamera;

    void Start(){
        Cursor.SetCursor(targetCursor, new Vector3(0,0,0), CursorMode.ForceSoftware);
    }
    void Update(){
        //float oi = myCamera.orthographicSize/6.5f;
        /*
        float oi = myCamera.transform.position.z - 10f;
        transform.localScale = new Vector3(oi,oi,0);
        Vector3 cursorPos = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f));
        transform.position = cursorPos;
        */
        if(Input.GetMouseButtonDown(0)){
            Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.ForceSoftware);
        }else if(Input.GetMouseButtonUp(0)){
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}