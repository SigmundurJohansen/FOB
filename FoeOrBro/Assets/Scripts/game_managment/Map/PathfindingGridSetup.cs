using UnityEngine;
#pragma warning disable 0649

public class PathfindingGridSetup : MonoBehaviour
{
    public static PathfindingGridSetup Instance { private set; get; }
    private Vector3 originVector = new Vector3(0, 0, 0);
    public Grid<GridNode> pathfindingGrid;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateGrid(int _x, int _y)
    {
        pathfindingGrid = new Grid<GridNode>(_x, _y, 0.32f, originVector, (Grid<GridNode> grid, int x, int y) => new GridNode(grid, x, y));
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            printPos();
        }
    }

    public void printPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Debug.Log(mousePos);
    }
}
