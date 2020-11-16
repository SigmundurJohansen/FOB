using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RoamSystem : ComponentSystem
{
    float checkRoamingTimer = 1;
    protected override void OnUpdate()
    {
        checkRoamingTimer -= UnityEngine.Time.deltaTime;
        Entities.WithNone<DestinationComponent>().ForEach((Entity entity, ref MovementComponent _move, ref Translation _trans, ref IDComponent _id, ref RoamingComponent _roam, ref StateComponent _state) =>
        {
            if (_state.state == 1)
            {
                PostUpdateCommands.RemoveComponent(entity, typeof(RoamingComponent));
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, new CombatComponent { });

            }
            checkRoamingTimer -= UnityEngine.Time.deltaTime;
            if (checkRoamingTimer < 0)
            {
                var randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(10f, 10f);


                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(_trans.Value, out int startX, out int startY);
                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out int endX, out int endY);

                while (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable() == false)
                {
                    Debug.Log("destination not valid");
                    randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(10f, 10f);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out endX, out endY);
                }
                Debug.Log("destination valid");
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                _move.isMoving = true;
                //Debug.Log("Roaming : " + endX + " " + endY);
                checkRoamingTimer = 5;
            }

        });
    }
}