using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class generate : MonoBehaviour
{
 worldgenerator myGenerator;

    void Awake(){
        myGenerator = GameObject.FindObjectOfType<worldgenerator>();
    }

    public void doGenerate(){
        myGenerator.Generate();
    }  
}
