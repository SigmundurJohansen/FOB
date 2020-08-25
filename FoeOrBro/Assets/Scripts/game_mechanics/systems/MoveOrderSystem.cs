using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MoveOrderSystem : ComponentSystem {

	Vector2 pointOne;
	Vector2 pointTwo;

    protected override void OnUpdate() 
	{
		if(SceneManager.GetActiveScene().name == "GameScene"){
			
        if (EventSystem.current.IsPointerOverGameObject())
        {
			
        }
        else
        {
			if (Input.GetMouseButtonDown(0)) {
				pointOne = WorldPosition();
			}
			if(Input.GetMouseButtonUp(0))
			{			
				pointTwo = WorldPosition(); 
				if((pointOne - pointTwo).magnitude > 0.2f)
				{
					Entities.ForEach((Entity entity, ref Translation _translation, ref Selected _selected) => {
					_selected.isSelected = false;
						if(_translation.Value.x > pointOne.x && _translation.Value.x < pointTwo.x && _translation.Value.y > pointOne.y && _translation.Value.y < pointTwo.y){
							_selected.isSelected = true;                       
						}if(_translation.Value.x < pointOne.x && _translation.Value.x > pointTwo.x && _translation.Value.y < pointOne.y && _translation.Value.y > pointTwo.y){
							_selected.isSelected = true;                       
						}
					});
				}else
				{
					float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
					Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
					PathfindingGridSetup.Instance.pathfindingGrid.GetXY(mousePosition, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize
					ValidateGridPosition(ref endX, ref endY);

					Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref Selected _selected) => {
						if(_selected.isSelected){
							PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out int startX, out int startY);
							ValidateGridPosition(ref startX, ref startY);
							EntityManager.AddComponentData(entity, new DestinationComponent {startPosition = new int2(startX, startY),endPosition = new int2(endX, endY)});
						}
					});
				}
			}
			if(Input.GetMouseButtonDown(1)){			
				Entities.ForEach((Entity entity, ref Translation _translation, ref Selected _selected) => {
					_selected.isSelected = false;
				});
			}
		}
		}
    }

    public Vector3 WorldPosition(){        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        return mousePos;
    }
    private void ValidateGridPosition(ref int x, ref int y) {
        x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth()-1);
        y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight()-1);
    }
}