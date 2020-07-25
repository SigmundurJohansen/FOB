using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MoveOrderSystem : ComponentSystem {

    protected override void OnUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
	        float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
	        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(mousePosition, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize
	        ValidateGridPosition(ref endX, ref endY);

	        Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation) => {
		        //Debug.Log("Add Component!");
		        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out int startX, out int startY);
		        ValidateGridPosition(ref startX, ref startY);
		        EntityManager.AddComponentData(entity, new DestinationComponent { 
			        startPosition = new int2(startX, startY), 
                    endPosition = new int2(endX, endY) 
		        });
	        });
        }
    }

    private void ValidateGridPosition(ref int x, ref int y) {
        x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth()-1);
        y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight()-1);
    }

}
