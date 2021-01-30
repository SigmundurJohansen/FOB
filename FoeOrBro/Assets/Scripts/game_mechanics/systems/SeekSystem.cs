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
    public static bool isDebug = false;

    [RequireComponentTag(typeof(SeekComponent))]
    [ExcludeComponent(typeof(HasTarget), typeof(FleeTag))]
    // Add HasTarget Component to Entities that have a Closest Target
    private struct AddComponentJob : IJobForEachWithEntity<Translation, MovementComponent, AttackComponent, StateComponent>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> closestTargetEntityArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation, ref MovementComponent _move, ref AttackComponent _attack, ref StateComponent _state)
        {
            if (closestTargetEntityArray.Length != 0)
                if (closestTargetEntityArray[index] != Entity.Null)
                {

                    // this needs changing (should not be using World.DefaultGameObjectInjectionWorld.EntityManager in jobs, only in main thread)
                    //DeathComponent dead = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<DeathComponent>(closestTargetEntityArray[index]);
                    //
                    //if (dead.isDead == false)
                    //{
                    Debug.Log("give hasTarget and go into combat");
                    entityCommandBuffer.AddComponent(index, entity, new HasTarget { targetEntity = closestTargetEntityArray[index] });
                    _state.state = 1;
                    _move.chaseTarget = true;
                    _attack.isAttacking = true;
                    //}
                }
        }
    }


    [RequireComponentTag(typeof(SeekComponent))]
    [ExcludeComponent(typeof(HasTarget))]
    [BurstCompile]
    private struct FindTargetSectorSystemJob : IJobForEachWithEntity<Translation, SectorEntity, DeathComponent>
    {
        [ReadOnly] public NativeMultiHashMap<int, SectorData> SectorMultiHashMap;
        public NativeArray<Entity> closestTargetEntityArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SectorEntity SectorEntity, ref DeathComponent _death)
        {
            float3 unitPosition = translation.Value;
            Entity closestTargetEntity = Entity.Null;
            float closestTargetDistance = float.MaxValue;
            bool isTargetDead = _death.isDead;
            int hashMapKey = SectorSystem.GetPositionHashMapKey(translation.Value);

            FindTarget(hashMapKey, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey + 1, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey - 1, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey + 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey - 1 + SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey + 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            FindTarget(hashMapKey - 1 - SectorSystem.SectorYMultiplier, unitPosition, SectorEntity, ref closestTargetEntity, ref closestTargetDistance, ref isTargetDead);
            if (closestTargetEntityArray.Length != 0)
            {
                if (!isTargetDead)
                    closestTargetEntityArray[index] = closestTargetEntity;
            }
        }

        private void FindTarget(int hashMapKey, float3 unitPosition, SectorEntity SectorEntity, ref Entity closestTargetEntity, ref float closestTargetDistance, ref bool isTargetDead)
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
                            isTargetDead = SectorData.isDead;
                        }
                        else
                        {
                            if (math.distancesq(unitPosition, SectorData.position) < closestTargetDistance)
                            {
                                // This target is closer
                                closestTargetEntity = SectorData.entity;
                                closestTargetDistance = math.distancesq(unitPosition, SectorData.position);
                                isTargetDead = SectorData.isDead;
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
        EntityQuery targetQuery = GetEntityQuery(typeof(Kobolt), ComponentType.ReadOnly<Translation>(), ComponentType.Exclude<DoNotTarget>());

        EntityQuery unitQuery = GetEntityQuery(typeof(SeekComponent), ComponentType.Exclude<HasTarget>());
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