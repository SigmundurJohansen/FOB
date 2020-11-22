using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Rendering;
using UnityEngine.SceneManagement;

[UpdateAfter(typeof(SeekSystem))]
public class FleeSystem : JobComponentSystem
{
    public static bool isDebug = false;
    private struct EntityWithTarget
    {
        public Entity entity;
        public float3 position;
        public Entity target;
        public bool isDead;
    }


    [RequireComponentTag(typeof(Kobolt))]
    [ExcludeComponent(typeof(FleeTag))]
    private struct AddNearbyEnemy : IJobForEachWithEntity<Translation, MovementComponent, StateComponent>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithTarget> closestAttackerEntityArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;

        public void Execute(Entity _entity, int _index, ref Translation _translation, ref MovementComponent _move, ref StateComponent _state)
        {
            if (closestAttackerEntityArray.Length != 0)
                if (closestAttackerEntityArray[_index].entity != Entity.Null)
                    if (closestAttackerEntityArray[_index].entity != _entity)
                    {

                        float3 oppositeDirection = math.normalize((_translation.Value - closestAttackerEntityArray[_index].position));
                        float oppositeForce = 5;
                        float3 destination = _translation.Value + oppositeDirection * oppositeForce;
                        if (isDebug)
                        {
                            Debug.Log("Give escape path");
                            Debug.Log("opposite = " + oppositeDirection);
                            Debug.Log("destination = " + destination);
                        }
                        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(destination, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize
                        ValidateGridPosition(ref endX, ref endY);

                        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(_translation.Value, out int startX, out int startY);
                        ValidateGridPosition(ref startX, ref startY);
                        _move.isMoving = true;
                        _move.chaseTarget = false;
                        if (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable())
                        {
                            entityCommandBuffer.AddComponent(_index, _entity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                            entityCommandBuffer.AddComponent(_index, _entity, new OrderComponent { hasOrders = true, orderType = 1 });
                        }
                        else
                            Debug.Log("Trapped");
                        entityCommandBuffer.AddComponent(_index, _entity, new FleeTag { });
                    }
                    else
                        Debug.Log("same entity! ");

        }
        private void ValidateGridPosition(ref int x, ref int y)
        {
            x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() - 1);
            y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight() - 1);
        }
    }

    [RequireComponentTag(typeof(Dragon))]
    [BurstCompile]
    private struct FindAttackerSectorSystemJob : IJobForEachWithEntity<Translation, SectorEntity>
    {
        [ReadOnly] public NativeMultiHashMap<int, SectorData> SectorMultiHashMap;
        public NativeArray<EntityWithTarget> closestAttackerEntityArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SectorEntity SectorEntity)
        {
            float3 unitPosition = translation.Value;
            Entity closestAttackerEntity = Entity.Null;
            float closestAttackerDistance = float.MaxValue;
            int hashMapKey = SectorSystem.GetPositionHashMapKey(translation.Value);

            FindAttacker(hashMapKey, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey + 1, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey - 1, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey + 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey - 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey + 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            FindAttacker(hashMapKey - 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestAttackerEntity, ref closestAttackerDistance);
            if (closestAttackerEntityArray.Length != 0)
            {
                EntityWithTarget attacker = new EntityWithTarget { entity = closestAttackerEntity, position = unitPosition };
                closestAttackerEntityArray[index] = attacker;
            }
        }

        private void FindAttacker(int hashMapKey, float3 unitPosition, SectorEntity SectorEntity, ref Entity closestAttackerEntity, ref float closestAttackerDistance)
        {
            SectorData SectorData;
            NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
            if (SectorMultiHashMap.TryGetFirstValue(hashMapKey, out SectorData, out nativeMultiHashMapIterator))
            {
                do
                {
                    if (closestAttackerEntity == Entity.Null)
                    {
                        // No target
                        closestAttackerEntity = SectorData.entity;
                        closestAttackerDistance = math.distancesq(unitPosition, SectorData.position);
                    }
                    else
                    {
                        if (math.distancesq(unitPosition, SectorData.position) < closestAttackerDistance)
                        {
                            // This target is closer
                            closestAttackerEntity = SectorData.entity;
                            closestAttackerDistance = math.distancesq(unitPosition, SectorData.position);
                        }
                    }

                } while (SectorMultiHashMap.TryGetNextValue(out SectorData, ref nativeMultiHashMapIterator));
            }
        }
    }
    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery attackerQuery = GetEntityQuery(typeof(Dragon), ComponentType.ReadOnly<Translation>());

        //EntityQuery unitQuery = GetEntityQuery(typeof(Kobolt));
        NativeArray<EntityWithTarget> closestAttackerEntityArray = new NativeArray<EntityWithTarget>(attackerQuery.CalculateEntityCount(), Allocator.TempJob);

        FindAttackerSectorSystemJob findAttackerSectorSystemJob = new FindAttackerSectorSystemJob
        {
            SectorMultiHashMap = SectorSystem.SectorMultiHashMap,
            closestAttackerEntityArray = closestAttackerEntityArray,
        };
        JobHandle jobHandle = findAttackerSectorSystemJob.Schedule(this, inputDeps);

        AddNearbyEnemy AddNearbyEnemySystemJob = new AddNearbyEnemy
        {
            closestAttackerEntityArray = closestAttackerEntityArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };
        jobHandle = AddNearbyEnemySystemJob.Schedule(this, jobHandle);


        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}


public class RemoveFleeJob : JobComponentSystem
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
        EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        JobHandle jobHandle = Entities.ForEach((Entity _entity, int entityInQueryIndex, in PathFollow _pathFollow, in Translation _translation, in FleeTag _flee) =>
        {
            if (_pathFollow.pathIndex == -1)
            {
                Debug.Log("Removing fleetag");
                entityCommandBuffer.RemoveComponent<FleeTag>(entityInQueryIndex, _entity);
            }
        }).Schedule(inputDeps);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

}