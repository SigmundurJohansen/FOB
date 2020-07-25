using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGridSetup : MonoBehaviour {

    public static PathfindingGridSetup Instance { private set; get; }
    //private Vector3 originVector = new Vector3(-.5f,-.5f,0);
    private Vector3 originVector = new Vector3(0,0,0);
    
    #pragma warning disable 0649
    [SerializeField] private PathfindingVisual pathfindingVisual;
    #pragma warning restore 0649
    public Grid<GridNode> pathfindingGrid;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pathfindingGrid = new Grid<GridNode>(10, 10, 1f, originVector, (Grid<GridNode> grid, int x, int y) => new GridNode(grid, x, y));

        //pathfindingGrid.GetGridObject(2, 0).SetIsWalkable(false);

        pathfindingVisual.SetGrid(pathfindingGrid);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = WorldPosition();// .5f    + (new Vector3(+1, +1) * pathfindingGrid.GetCellSize())
            //mousePosition.x += .5f;
            //mousePosition.y += .5f;
            GridNode gridNode = pathfindingGrid.GetGridObject(mousePosition);
            if (gridNode != null) {
                gridNode.SetIsWalkable(!gridNode.IsWalkable());
            }
        }
    }

    public Vector3 WorldPosition(){        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Debug.Log("WorldPosition: " + mousePos);
        //mousePos.z = transform.position.z;
        return mousePos;
    }
}
