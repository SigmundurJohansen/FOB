using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGenerateParam : MonoBehaviour
{
    worldgenerator myGenerator;

    void Awake(){
        myGenerator = GameObject.FindObjectOfType<worldgenerator>();
    }

    public void SetDeathLimit(int i){
        myGenerator.setDeath(i);
    }  
    public void SetBirthLimit(int i){
        myGenerator.setBirth(i);
    }  
    public void SetWidth(int i){
        myGenerator.setWidth(i);
    }  
    public void SetHeight(int i){
        myGenerator.setHeigth(i);
    }  
}
