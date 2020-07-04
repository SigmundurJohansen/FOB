using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour
{
    public Vector3 startingPosition;
    public Vector3 currentPosition;
    public Vector3 destination;
    public float unitSpeed = 2;
    public bool idle = false;
    public SpriteRenderer spriteRenderer;
    public Sprite sprite;

    void ChangeSprite()
    {
        spriteRenderer.sprite = sprite; 
    }
    
    void Start()
    {
        startingPosition = transform.position;
        
        destination = RandomPosition();
        ChangeSprite();
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.position;
        MoveToPosition(destination);
    }

    Vector3 RandomPosition(){
        return new Vector3(startingPosition.x + Random.Range(-10f, 10f), startingPosition.y + Random.Range(-10f,10f), -0.1f);
    }

    void MoveToPosition(Vector3 targetDestination){
        float x = 0,y = 0,z = -0.1f;
        Vector3 newPos = new Vector3(transform.position.x,transform.position.y,z);
        float minDistance = 1;
        bool first = false, second = false;

        //                          X
        if(Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(targetDestination.x)) > minDistance ){

            if(transform.position.x  < targetDestination.x){
                newPos.x = transform.position.x + unitSpeed * Time.deltaTime;            
            }
            if(transform.position.x  > targetDestination.x){
                newPos.x = transform.position.x - unitSpeed * Time.deltaTime;   
            }
        }else 
            first = true;

        //                          Y
        if(Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(targetDestination.y)) > minDistance ){
            if(transform.position.y < targetDestination.y){
                newPos.y = transform.position.y + unitSpeed * Time.deltaTime;    
            }
            if(transform.position.y > targetDestination.y){
                newPos.y = transform.position.y - unitSpeed * Time.deltaTime;  
            }
        }else
            second = true;

        transform.position = newPos;

        if(first && second)
            destination = RandomPosition();
    }
}
