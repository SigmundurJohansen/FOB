﻿using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine.SceneManagement;


[UpdateAfter(typeof(PathfindingSystem))]

public class PathFollowSystem : JobComponentSystem
{

    private Unity.Mathematics.Random random;
    //float gridCellSize = 0.32f;

    protected override void OnCreate()
    {
        random = new Unity.Mathematics.Random(56);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        return Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer,ref DeathComponent _dead, ref Translation translation, ref Rotation rot, ref PathFollow pathFollow, ref MovementComponent _move, ref IDComponent _id) =>
        {
            rot.Value = new quaternion(0, 0, 0, 1);
            if (pathFollow.pathIndex >= 0 && _move.isMoving && _dead.isDead == false)
            {
                // Has path to follow
                PathPosition pathPosition = pathPositionBuffer[pathFollow.pathIndex];
                float3 targetPosition = new float3((pathPosition.position.x + .5f) * 0.32f, (pathPosition.position.y + .5f) * 0.32f, 0);
                float3 moveDir = math.normalizesafe(targetPosition - translation.Value);
                translation.Value += moveDir * _move.speed * deltaTime;
                float3 myXy = translation.Value;
                //Debug.Log(_id.id);
                //GameController.Instance.SetPosition(_id.id, myXy.x, myXy.y);
                if (math.distance(translation.Value, targetPosition) < .1f)
                {
                    // Next waypoint
                    pathFollow.pathIndex--;
                }
            }
            else
                _move.isMoving = false;
        }).Schedule(inputDeps);
    }

    private void ValidateGridPosition(ref int x, ref int y)
    {
        x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() - 1);
        y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight() - 1);
    }
}

[UpdateAfter(typeof(PathFollowSystem))]
[DisableAutoCreation]
public class PathFollowGetNewPathSystem : JobComponentSystem
{

    private Unity.Mathematics.Random random;
    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        random = new Unity.Mathematics.Random(56);
        if (SceneManager.GetActiveScene().name == "GameScene")
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int mapWidth = PathfindingGridSetup.Instance.pathfindingGrid.GetWidth();
        int mapHeight = PathfindingGridSetup.Instance.pathfindingGrid.GetHeight();
        float3 originPosition = float3.zero;
        float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(this.random.NextUInt(1, 10000));
        EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        JobHandle jobHandle = Entities.WithNone<DestinationComponent>().ForEach((Entity entity, int entityInQueryIndex, in PathFollow pathFollow, in Translation translation) =>
        {
            if (pathFollow.pathIndex == -1)
            {
                GetXY(translation.Value + new float3(1, 1, 0) * cellSize, cellSize, out int startX, out int startY);
                ValidateGridPosition(ref startX, ref startY, mapWidth, mapHeight);
                int endX = random.NextInt(0, mapWidth);
                int endY = random.NextInt(0, mapHeight);
                Debug.Log("give destination pathfollow system");
                entityCommandBuffer.AddComponent(entityInQueryIndex, entity, new DestinationComponent
                {
                    startPosition = new int2(startX, startY),
                    endPosition = new int2(endX, endY)
                });
            }
        }).Schedule(inputDeps);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

    private static void ValidateGridPosition(ref int x, ref int y, int width, int height)
    {
        x = math.clamp(x, 0, width - 1);
        y = math.clamp(y, 0, height - 1);
    }

    private static void GetXY(float3 worldPosition, float cellSize, out int x, out int y)
    {
        x = (int)math.floor(worldPosition.x / cellSize);
        y = (int)math.floor(worldPosition.y / cellSize);
    }
}
