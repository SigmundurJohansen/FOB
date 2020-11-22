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
        Entities.WithNone<DestinationComponent>().WithNone<FleeTag>().ForEach((Entity entity, ref DeathComponent _death, ref MovementComponent _move, ref Translation _trans, ref IDComponent _id, ref RoamingComponent _roam, ref StateComponent _state) =>
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
                randomDir = randomDir + new Vector3(_trans.Value.x, _trans.Value.y, _trans.Value.z);

                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(_trans.Value, out int startX, out int startY);
                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out int endX, out int endY);
                // warning!! very ugly! dont look!!
                while (endX <= 0 || endY <= 0 || endX >= PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() || endY >= PathfindingGridSetup.Instance.pathfindingGrid.GetHeight())
                {
                    randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(10f, 10f);
                    randomDir = randomDir + new Vector3(_trans.Value.x, _trans.Value.y, _trans.Value.z);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out endX, out endY);
                }
                while (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable() == false)
                {
                    randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(10f, 10f);
                    randomDir = randomDir + new Vector3(_trans.Value.x, _trans.Value.y, _trans.Value.z);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out endX, out endY);
                    while (endX <= 0 || endY <= 0 || endX >= PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() || endY >= PathfindingGridSetup.Instance.pathfindingGrid.GetHeight())
                    {
                        randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * UnityEngine.Random.Range(10f, 10f);
                        randomDir = randomDir + new Vector3(_trans.Value.x, _trans.Value.y, _trans.Value.z);
                        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(randomDir, out endX, out endY);
                    }
                }
                if (!_death.isDead)
                {
                    World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                    _move.isMoving = true;
                }
                checkRoamingTimer = 5;
            }
        });
    }
}