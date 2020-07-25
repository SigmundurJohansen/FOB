using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGridSetup : MonoBehaviour {

    public static PathfindingGridSetup Instance { private set; get; }

    [SerializeField] private PathfindingVisual pathfindingVisual;
    public Grid<GridNode> pathfindingGrid;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pathfindingGrid = new Grid<GridNode>(20, 20, 1f, Vector3.zero, (Grid<GridNode> grid, int x, int y) => new GridNode(grid, x, y));

        pathfindingGrid.GetGridObject(2, 0).SetIsWalkable(false);

        pathfindingVisual.SetGrid(pathfindingGrid);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = WorldPosition() + (new Vector3(+1, +1) * pathfindingGrid.GetCellSize() * .5f);
            GridNode gridNode = pathfindingGrid.GetGridObject(mousePosition);
            if (gridNode != null) {
                gridNode.SetIsWalkable(!gridNode.IsWalkable());
            }
        }
    }

    public Vector3 WorldPosition(){        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        //mousePos.z = transform.position.z;
        return mousePos;
    }
}
