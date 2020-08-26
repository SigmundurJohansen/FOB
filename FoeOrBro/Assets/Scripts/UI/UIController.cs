using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class UIController : MonoBehaviour
{
    [SF] public GameObject entityListView;
    private bool entityIsActive = false;
    [SF] public GameObject map;
    private bool mapIsActive = false;

    void Awake()
    {
        entityListView.SetActive(false);
        map.SetActive(false);
    }
    void Update()
    {
        
        if(Input.GetKeyDown("a"))
        {
            //rectTransform.Set
            if(entityIsActive)
            {
                entityListView.SetActive(false);
                entityIsActive = false;
            }else
            {
                entityListView.SetActive(true);
                entityIsActive = true;
                entityListView.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }
        if(Input.GetKeyDown("m"))
        {
            //rectTransform.Set
            if(mapIsActive)
            {
                map.SetActive(false);
                mapIsActive = false;
            }else
            {
                map.SetActive(true);
                mapIsActive = true;
                map.GetComponent<RectTransform>().SetAsLastSibling();
            }
        }
    }
}
