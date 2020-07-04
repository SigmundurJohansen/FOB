using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public Vector3 startingPosition;
    public Vector3 currentPosition;
    public Vector3 destination;
    public float unitSpeed = 2;
    public bool idle = false;
    public SpriteRenderer spriteRenderer;
    public Sprite sprite;
    public string unitName = "unit";

    
    public float currentHealth { get;set;}
    public float Health = 100;
    public int Mana = 100;
    public int currentMana;
    public int Strength = 10, Dexterity = 10, Constitution = 10, Intelligence = 10, Wisdom = 10, Charisma = 10;

    public void TakeDamage(float damage){
        Health -= damage;
        Debug.Log(Health);
        if(Health <= 0)
            Destroy(gameObject);
    }

    void ChangeSprite()
    {
        spriteRenderer.sprite = sprite; 
    }
    
    void Start()
    {
        currentHealth = Health;
        currentMana = Mana;
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
        float z = -0.1f;
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
    public void Damage(float damageAmount){
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
            Destroy(gameObject);
    }
    public string Name(){
        return name;
    }
}
