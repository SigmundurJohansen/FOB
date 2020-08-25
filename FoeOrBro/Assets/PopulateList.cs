using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.Entities;
using SF = UnityEngine.SerializeField;

public class PopulateList : MonoBehaviour
{
    private EntityManager entityManager;
    private GameObject instance;
    [SF] private GameObject contentPrefab;
    [SF] private GameObject viewPrefab;
    // Start is called before the first frame update

    public void Awake() 
    {
        //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        
    }
}
