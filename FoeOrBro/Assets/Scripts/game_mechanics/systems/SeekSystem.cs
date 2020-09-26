
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public class SeekSystem : SystemBase
{
    private EntityQuery TargetQuery;
    private EntityQuery PickerQuery;
    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


        PickerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] {
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadOnly<PerceptionComponent>(),
                    //ComponentType.ReadOnly<AggroLocation>(),
                    ComponentType.ReadWrite<TargetComponent>()
                }
        });
    }

    protected override void OnUpdate()
    {
        //PickerQuery.AddDependency(Dependency);
        //TargetQuery.AddDependency(Dependency);

        //int targetN = TargetQuery.CalculateEntityCount();
        //var targetables = GetComponentDataFromEntity<Targetable>(true);

        //var targetPositions = TargetQuery.ToComponentDataArrayAsync<LocalToWorld>(Allocator.TempJob, out var tposJH);
        //ar targetIDs = TargetQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle tidJH);
        EntityCommandBuffer.Concurrent entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        //.WithStoreEntityQueryInField(ref TargetQuery).
        var hashTargetsJobHandle = Entities.WithNone<DestinationComponent>().WithAll<FactionComponent, TargetableComponent>()
        .ForEach((Entity entity, int entityInQueryIndex, ref LocalToWorld _localToWorld, ref TargetComponent _target) =>
        {
            if (_target.Value == Entity.Null)
            {
                Debug.Log("i'm in!");
                var position = _localToWorld.Position;
                float3 vec = position;
                vec += 20;
                Debug.Log(vec);
                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(vec, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize

                if (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable())
                {
                    PathfindingGridSetup.Instance.pathfindingGrid.GetXY(_localToWorld.Position, out int startX, out int startY);
                    entityCommandBuffer.AddComponent(entityInQueryIndex, entity, new DestinationComponent
                    {
                        startPosition = new int2(startX, startY),
                        endPosition = new int2(endX, endY)
                    });
                }
                //_target.Value = entity;
            }
        }).Schedule(Dependency);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(hashTargetsJobHandle);

        /*
        Entities.ForEach((Entity entity, ref RaceComponent _race, ref IDComponent _id, ref TargetComponent _target, ref StateComponent _state, ref PerceptionComponent _perc) =>
        {
            if (_state.state == 0) //seek
            {

            }
        });
         */

    }

    [BurstCompile]
    struct IdentifyBestTargetChunkJob : IJobChunk
    {
        public float CellSize;

        //Picker components
        [ReadOnly] public ArchetypeChunkComponentType<LocalToWorld> PickerLocalToWorld;
        [ReadOnly] public ArchetypeChunkComponentType<PerceptionComponent> PickerAggroRadii;
        [ReadOnly] public ArchetypeChunkComponentType<FactionComponent> PickerTeams;
        public ArchetypeChunkComponentType<TargetComponent> PickerTargets;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            for (int picker = 0; picker < chunk.Count; picker++)
            {
            }
        }
    }
}
