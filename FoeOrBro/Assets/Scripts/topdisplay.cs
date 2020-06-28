using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class topdisplay : MonoBehaviour
{

    public Text heightText;
    public Text widthText;
    public Text deathText;
    public Text birthText;

    private worldgenerator myGenerator;

    void Awake(){
        myGenerator = GameObject.FindObjectOfType<worldgenerator>();
    }
    public void Update(){
        
        widthText.text = "Width : " + myGenerator.width;
        heightText.text = "Height : " +myGenerator.height;
        deathText.text = "Death Limit : " +myGenerator.deathLimit;
        birthText.text = "Birth Limit : " +myGenerator.birthLimit;
    }

}
