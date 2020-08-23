using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class UIController : MonoBehaviour
{
    private bool isActive = true;
    [SF] public GameObject entityListView;

    void Update()
    {
        
        if(Input.GetKeyDown("a"))
        {
            //rectTransform.Set
            if(isActive){
                entityListView.SetActive(false);
                isActive = false;
            }
            else
            {
                entityListView.SetActive(true);
                isActive = true;
            }
        }
    }
}
