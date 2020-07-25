using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using System.Threading;
using Unity.Burst;
/*
public class MovementSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }
// IJobForEachWithEntity
    [BurstCompile]
    struct MovementSystemJob : IJobForEachWithEntity<Translation, MovementComponent>
    {
        [ReadOnly] public EntityCommandBuffer CommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<DestinationComponent> Destination;
        public float DeltaTime;

       
       public void Execute(Entity entity, int index, ref Translation _translation, ref MovementComponent _movement)
       {
           float3 dontMove = new float3(0,0,0);

            if(Destination.Exists(entity)){
                if(_movement.isMoving){
                    if( Mathf.Abs(_movement.direction.x - _translation.Value.x) < 2 || Mathf.Abs(_movement.direction.y - _translation.Value.y) < 2)
                    {
                        //CommandBuffer.RemoveComponent<DestinationComponent>(entity);
                        _movement.isMoving = false;
                        return;
                    }
                    var heading = math.normalizesafe(_movement.direction - _translation.Value);
                    var distance = heading.x * heading.x + heading.y * heading.y;
                    var direction = heading / distance;
                    _translation.Value += direction * _movement.speed * DeltaTime; 
                }else{
                    /*                   
                    if(Mathf.Abs(_destination.destination.x - _translation.Value.x) > 2 || Mathf.Abs(_destination.destination.y - _translation.Value.y) > 2)
                    {
                        CommandBuffer.AddComponent(entity, new DestinationComponent());
                        _movement.isMoving = true;
                    }
                    *//*
                }
            }
       }
   }

protected override JobHandle OnUpdate(JobHandle inputDependencies)
   {
       var job = new MovementSystemJob()
       {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            Destination = GetComponentDataFromEntity<DestinationComponent>(),
            DeltaTime = UnityEngine.Time.deltaTime
       };
       return job.Schedule(this, inputDependencies);
   }
}

 */