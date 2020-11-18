using System;
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

    public Grid(int _width, int _height, float _cellSize, Vector3 _originPosition, Func<Grid<TGridObject>, int, int, TGridObject> _createGridObject) {
        this.width = _width;
        this.height = _height;
        this.cellSize = _cellSize;
        this.originPosition = _originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = _createGridObject(this, x, y);
            }
        }
        bool showDebug = false;
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

    public Vector3 GetWorldPosition(int _x, int _y) {
        return new Vector3(_x, _y) * cellSize + originPosition;  //   new Vector3(x, y) * cellSize + originPosition
    }

    public void GetXY(Vector3 _vector3, out int _x, out int _y) {
        float x = _vector3.x;
        float y = _vector3.y;
        if(_vector3.x >= this.width)
            x  = this.width-10;
        if(_vector3.x <= 0)
            x  = 0;
        if(_vector3.y >= this.height)
            y  = this.height-10;
        if(_vector3.y <= 0)
            y  = 0;
        _x = Mathf.FloorToInt(x / cellSize);
        _y = Mathf.FloorToInt(y / cellSize);//  / cellSize
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

    public void TriggerGridObjectChanged(int _x, int _y) {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = _x, y = _y });
    }


    public TGridObject GetGridObject(int _x, int _y) {
        if (_x >= 0 && _y >= 0 && _x < width && _y < height) {
            return gridArray[_x, _y];
        } else {
            Debug.Log("Grid x is: "+ PathfindingGridSetup.Instance.pathfindingGrid.GetWidth()+ "Grid Y is: " + PathfindingGridSetup.Instance.pathfindingGrid.GetHeight());
            Debug.Log("GetGridObject(int,int) " + _x +" , " +_y +" returning default");
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 _worldPosition) {
        int x, y;
        GetXY(_worldPosition, out x, out y);
        //Debug.Log("after GetXY(Vector3)" + x +"," +y);
        return GetGridObject(x, y);
    }
    
    public TextMeshPro CreateGridText( Vector3 _localPosition, int _x, int _y){      
        Vector3 offset = new Vector3(this.cellSize / 2, this.cellSize / 2, 0);
        GameObject go = new GameObject();
        Transform transform = go.transform;
        TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
        textMesh.transform.localPosition = _localPosition + offset;
        textMesh.autoSizeTextContainer = true;
        textMesh.fontSize = 1;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.enableWordWrapping = false; 
        textMesh.SetText(_x +"," + _y);
        return textMesh;
    }
}
