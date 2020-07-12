using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseCursor : MonoBehaviour {
    private SpriteRenderer myRend;
    public Sprite handCursor;
    public Sprite normalCursor;
    public Camera myCamera;

    void Start(){
        Cursor.visible = false;
        myRend = GetComponent<SpriteRenderer>();
    }
    void Update(){
        float oi = myCamera.orthographicSize/2.5f;
        transform.localScale = new Vector3(oi,oi,0);
        Vector3 cursorPos = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        transform.position = cursorPos;

        if(Input.GetMouseButtonDown(0)){
            myRend.sprite = handCursor;
        }else if(Input.GetMouseButtonUp(0)){
            myRend.sprite = normalCursor;
        }
    }
}