
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

public class UnitMoveToTargetSystem : ComponentSystem
{
    float moveTimer = 1;
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.ForEach((Entity unitEntity, ref HasTarget _hasTarget, ref Translation translation, ref IDComponent _id, ref MovementComponent _move) =>
        {
            if (entityManager.Exists(_hasTarget.targetEntity))
            {
                Translation targetTranslation = entityManager.GetComponentData<Translation>(_hasTarget.targetEntity);
                IDComponent targetID = entityManager.GetComponentData<IDComponent>(_hasTarget.targetEntity);

                if (math.distancesq(translation.Value, targetTranslation.Value) < .1f)
                {
                    // Close to target, destroy it
                    //Debug.Log("damage target id: " + targetID.id);
                    //GameController.Instance.DamageUnit(targetID.id, 20, 0);
                    //PostUpdateCommands.DestroyEntity(hasTarget.targetEntity);
                    //PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                }
                else if (_move.chaseTarget == true)
                {
                    moveTimer -= UnityEngine.Time.deltaTime;
                    if (moveTimer < 0)
                    {
                        _move.isMoving = true;

                        //float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
                        //float moveSpeed = 1f;
                        //translation.Value += targetDir * moveSpeed * deltaTime;
                        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(targetTranslation.Value, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize
                        ValidateGridPosition(ref endX, ref endY);
                        if (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable())
                        {
                            PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out int startX, out int startY);
                            ValidateGridPosition(ref startX, ref startY);
                            EntityManager.AddComponentData(unitEntity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                        }
                        else
                        {
                            Debug.Log("target is not reachable!!");
                        }
                        moveTimer = 2;
                    }
                }
            }
            else
            {
                // Target Entity already destroyed
                PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                Debug.Log("remove Entity hastarget");
            }
        });
    }
    private void ValidateGridPosition(ref int x, ref int y)
    {
        x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() - 1);
        y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight() - 1);
    }
}
