using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitOnClick : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame(){
        
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit ();
        Debug.Log("Quit!");
    }
}
