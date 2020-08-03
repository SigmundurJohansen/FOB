using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Grid<TGridObject> {
    public const int sortingOrderDefault = 5;
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
        bool showDebug = true;
        if (showDebug) {
            TextMeshPro[,] debugTextArray = new TextMeshPro[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {                    
                    debugTextArray[x,y] = CreateGridText( GetWorldPosition(x, y),x, y);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 1000f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 1000f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                //debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;  //   new Vector3(x, y) * cellSize + originPosition
    }

    public void GetXY(Vector3 _vector3, out int x, out int y) {
        
        x = Mathf.FloorToInt(_vector3.x / cellSize);
        y = Mathf.FloorToInt(_vector3.y / cellSize);//  / cellSize
    }
    /*
    public void SetGridObject(int x, int y, TGridObject value) {
        Debug.Log("SetGridObject");
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }
    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    } 
    */

    public void TriggerGridObjectChanged(int x, int y) {
        //Debug.Log("TriggerGridObjectChanged");
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }


    public TGridObject GetGridObject(int x, int y) {
        //Debug.Log("GetGridObject(int,int)" + x +"," +y);
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            Debug.Log("GetGridObject(int,int)" + x +"," +y +"returning default");
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        //Debug.Log("after GetXY(Vector3)" + x +"," +y);
        return GetGridObject(x, y);
    }
    
    public TextMeshPro CreateGridText( Vector3 localPosition, int x, int y){      
        Vector3 offset = new Vector3(this.cellSize / 2, this.cellSize / 2, 0);
        GameObject go = new GameObject();
        Transform transform = go.transform;
        TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
        textMesh.transform.localPosition = localPosition + offset;
        textMesh.autoSizeTextContainer = true;
        textMesh.fontSize = 1;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.enableWordWrapping = false; 
        textMesh.SetText(x +"," + y);
        return textMesh;
    }
}
