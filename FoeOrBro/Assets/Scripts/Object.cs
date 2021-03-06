﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public float currentHealth { get;set;}
    private string myName = "book";
    public bool isSelected { get;set;}

    public void Damage(float damageAmount){
        currentHealth -= 10;
        Debug.Log("Object's current health: " + currentHealth);
        if(currentHealth <= 0)
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string Name(){
        return myName;
    }
}
