using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Vector2 mapSize;

    public void GenerateMap(){
        for (int x = 0; x < mapSize.x; x++){
            for(int y = 0; y < mapSize.y; y++){
                //Vector3 tilePosition = new Vector3(-)
            }
        }
    }

}
