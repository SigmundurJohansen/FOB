
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Specialized;
using System.Collections;
using UnityEngine.UI;

public class worldgenerator : MonoBehaviour
{
    public int width;
    public int height;
    public Slider randomSlider;
    public Slider deathSlider;
    public Slider birthSlider;
    public Slider widthSlider;
    public Slider heightSlider;
       
    [Range(0,100)]
    public int randomFilPercent;
    [Range(1,8)]
    public int birthLimit;
    [Range(1,8)]
    public int deathLimit;
    [Range(1,10)]
    public int repetitions;
    int[,] map;

    private int[,] terrainMap;
    public Vector3Int tmapSize;

    public Tilemap topMap;
    public Tilemap botMap;

    public Tile topTile;
    public Tile botTile;

    void Start()
    {
        GenerateMap(repetitions);
    }

    void GenerateMap(int repeat)
    {
        clearMap(false);

        if(terrainMap == null)
        {
            terrainMap = new int[width,height];
            RandomFilMap();
        }

        for(int i = 0;i < repeat;i++){
            terrainMap = generateTileOnPosition(terrainMap);
        }

        for(int x = 0;x < width;x++){
            for(int y = 0;y < height;y++){
                if(terrainMap[x,y] == 1){
                    topMap.SetTile(new Vector3Int(-x + width /2, -y + height /2, 0), topTile);
                    botMap.SetTile(new Vector3Int(-x + width /2, -y + height /2, 0), botTile);
                }
            }
        }

    }

    public int [,] generateTileOnPosition(int[,] oldMap){
        int[,] newMap = new int[width,height];
        int neighb;

        BoundsInt myB = new BoundsInt(-1,-1,0,3,3,1);

        for(int x = 0;x < width; x++)
        {
            for (int y = 0; y <height; y++)
            {
                neighb = 0;
                foreach(var b in myB.allPositionsWithin){
                    if(b.x==0 && b.y == 0) continue;
                    if(x+b.x >= 0 && x+b.x < width && y+b.y >= 0 && y+b.y < height){
                        neighb += oldMap[x + b.x, y + b.y];
                    }else{
                        neighb++;
                    }
                }
                if(oldMap[x,y] ==1){
                    if(neighb < deathLimit) newMap[x,y] = 0;
                    else{
                        newMap[x,y] = 1;
                    }
                }
                if(oldMap[x,y] == 0){
                    if(neighb > birthLimit) newMap[x,y] =1;
                    else{
                        newMap[x,y] = 0;
                    }
                }
            }
        }
        return newMap;
    }


    public void clearMap(bool complete){
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        if(complete){
            terrainMap = null;
        }
    }
    public void setDeath(int newDeath){
        deathLimit = newDeath;
    }    
    public void setBirth(int newBirth){
        birthLimit = newBirth;
    }    
    public void setWidth(int newWidth){
        width = newWidth;
    }    
    public void setHeigth(int newHeight){
        height = newHeight;
    }    

    public void RandomFilMap()
    {
        System.Random prng = new System.Random(5.GetHashCode());

        for(int x = 0;x < width; x++)
        {
            for (int y = 0; y <height; y++)
            {
                terrainMap[x, y] =  (prng.Next(0, 100) < randomFilPercent) ? 1 : 0;
            }
        }
    }

    public void Generate(){
        GenerateMap(repetitions);
    }
    public void clearMap(){
        clearMap(true);
    }

    void Update(){

        deathLimit = (int)deathSlider.value; 
        randomFilPercent = (int)randomSlider.value; 
        birthLimit = (int)birthSlider.value; 
        width = (int)widthSlider.value; 
        height = (int)heightSlider.value; 
        if(Input.GetMouseButtonDown(0))
        {
            //Generate();
        }

        if(Input.GetMouseButtonDown(1))
        {
            //clearMap();
        }
    }
/* 
    public void SaveAssetMap()
    {
        string saveName = "map" +width +"x" + height;
        var mf = GameObject.Find("Grid");
        if(mf){
            var savePath = "Assets/" + saveName +".prefab";
            if(PrefabUtility.SaveAsPrefabAsset(mf,savePath)){
                EditorUtility.DisplayDialog("tilemap saved", "your tilemap was saved under" + savePath, "Continue");
            }else{
                
                EditorUtility.DisplayDialog("tilemap not saved", "error occurred", "Continue");
            }
        }
    }
    */
}
