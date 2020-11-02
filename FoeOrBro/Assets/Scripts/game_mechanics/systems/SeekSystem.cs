using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.SceneManagement;

[UpdateAfter(typeof(SectorSystem))]
public class SeekSystem : JobComponentSystem
{
    private struct EntityWithPosition
    {
        public Entity entity;
        public float3 position;
    }

    [RequireComponentTag(typeof(Dragon))]
    [BurstCompile]
    // Fill single array with Target Entity and Position
    private struct FillArrayEntityWithPositionJob : IJobForEachWithEntity<Translation>
    {

        public NativeArray<EntityWithPosition> targetArray;

        public void Execute(Entity entity, int index, ref Translation translation)
        {
            if (targetArray.Length != 0)
            {
                targetArray[index] = new EntityWithPosition
                {
                    entity = entity,
                    position = translation.Value
                };
            }
        }
    }

    [RequireComponentTag(typeof(Dragon))]
    [ExcludeComponent(typeof(HasTarget))]
    [BurstCompile]
    // Find Closest Target
    private struct FindTargetBurstJob : IJobForEachWithEntity<Translation>
    {

        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targetArray;
        public NativeArray<Entity> closestTargetEntityArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation)
        {
            float3 unitPosition = translation.Value;
            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;

            for (int i = 0; i < targetArray.Length; i++)
            {
                // Cycling through all target entities
                EntityWithPosition targetEntityWithPosition = targetArray[i];

                if (closestTargetEntity == Entity.Null)
                {
                    // No target
                    Debug.Log("no target");
                    closestTargetEntity = targetEntityWithPosition.entity;
                    closestTargetDistance = math.distancesq(unitPosition, targetEntityWithPosition.position);
                }
                else
                {
                    if (math.distancesq(unitPosition, targetEntityWithPosition.position) < closestTargetDistance)
                    {
                        // This target is closer
                        Debug.Log("target is closer");
                        closestTargetEntity = targetEntityWithPosition.entity;
                        closestTargetDistance = math.distancesq(unitPosition, targetEntityWithPosition.position);
                    }
                }
            }
            if (closestTargetEntityArray.Length != 0)
                closestTargetEntityArray[index] = closestTargetEntity;
        }

    }

    [RequireComponentTag(typeof(Dragon))]
    [ExcludeComponent(typeof(HasTarget))]
    // Add HasTarget Component to Entities that have a Closest Target
    private struct AddComponentJob : IJobForEachWithEntity<Translation>
    {

        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> closestTargetEntityArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation)
        {
            if (closestTargetEntityArray.Length != 0)
                if (closestTargetEntityArray[index] != Entity.Null)
                {
                    Debug.Log("give hastarget");
                    entityCommandBuffer.AddComponent(index, entity, new HasTarget { targetEntity = closestTargetEntityArray[index] });
                }
        }

    }


    [RequireComponentTag(typeof(Dragon))]
    [ExcludeComponent(typeof(HasTarget))]
    [BurstCompile]
    private struct FindTargetSectorSystemJob : IJobForEachWithEntity<Translation, SectorEntity>
    {

        [ReadOnly] public NativeMultiHashMap<int, SectorData> SectorMultiHashMap;
        public NativeArray<Entity> closestTargetEntityArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SectorEntity SectorEntity)
        {
            float3 unitPosition = translation.Value;
            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            int hashMapKey = SectorSystem.GetPositionHashMapKey(translation.Value);

            FindTarget(hashMapKey, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey + 1, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey - 1, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey + 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey - 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey + 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            FindTarget(hashMapKey - 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance);
            if (closestTargetEntityArray.Length != 0)
                closestTargetEntityArray[index] = closestTargetEntity;
        }

        private void FindTarget(int hashMapKey, float3 unitPosition, SectorEntity SectorEntity, ref Entity closestTargetEntity, ref float closestTargetDistance)
        {
            SectorData SectorData;
            NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
            if (SectorMultiHashMap.TryGetFirstValue(hashMapKey, out SectorData, out nativeMultiHashMapIterator))
            {
                do
                {
                    if (SectorEntity.typeEnum != SectorData.SectorEntity.typeEnum)
                    {
                        if (closestTargetEntity == Entity.Null)
                        {
                            // No target
                            closestTargetEntity = SectorData.entity;
                            closestTargetDistance = math.distancesq(unitPosition, SectorData.position);
                        }
                        else
                        {
                            if (math.distancesq(unitPosition, SectorData.position) < closestTargetDistance)
                            {
                                // This target is closer
                                closestTargetEntity = SectorData.entity;
                                closestTargetDistance = math.distancesq(unitPosition, SectorData.position);
                            }
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
        EntityQuery targetQuery = GetEntityQuery(typeof(Kobolt), ComponentType.ReadOnly<Translation>());

        EntityQuery unitQuery = GetEntityQuery(typeof(Dragon), ComponentType.Exclude<HasTarget>());
        NativeArray<Entity> closestTargetEntityArray = new NativeArray<Entity>(unitQuery.CalculateEntityCount(), Allocator.TempJob);

        FindTargetSectorSystemJob findTargetSectorSystemJob = new FindTargetSectorSystemJob
        {
            SectorMultiHashMap = SectorSystem.SectorMultiHashMap,
            closestTargetEntityArray = closestTargetEntityArray,
        };
        JobHandle jobHandle = findTargetSectorSystemJob.Schedule(this, inputDeps);

        // Add HasTarget Component to Entities that have a Closest Target
        AddComponentJob addComponentJob = new AddComponentJob
        {
            closestTargetEntityArray = closestTargetEntityArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };
        jobHandle = addComponentJob.Schedule(this, jobHandle);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

}