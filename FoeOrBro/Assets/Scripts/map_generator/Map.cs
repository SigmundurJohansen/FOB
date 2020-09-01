using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Map{

    public Tile[,] mapTiles;
    public int mapWidth;
    public int mapHeight;

    public Map(int _width, int _height, Tile[,] _tiles)
    {
        mapWidth = _width;
        mapHeight = _height;
        mapTiles = _tiles;

    }
}