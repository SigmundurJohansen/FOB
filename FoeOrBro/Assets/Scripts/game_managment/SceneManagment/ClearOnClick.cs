using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearOnClick : MonoBehaviour
{
    worldgenerator myGenerator;

    void Awake(){
        myGenerator = GameObject.FindObjectOfType<worldgenerator>();
    }

    public void doClear(){
        myGenerator.clearMap();
    }  
}
