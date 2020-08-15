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

public class MovementSystem : JobComponentSystem
{
    // IJobForEachWithEntity
    #pragma warning disable 0618
    [BurstCompile]
    struct MovementSystemJob : IJobForEachWithEntity<RigidBody, MovementComponent>
    {
        public float DeltaTime;
       
        public void Execute(Entity entity, int index, ref RigidBody _rigidbody , ref MovementComponent _movement)
        {
            _rigidbody.velocity = _movement.speed * DeltaTime;            
        }
   }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new MovementSystemJob()
        {
            DeltaTime = UnityEngine.Time.deltaTime
        };
        return job.Schedule(this, inputDependencies);
    }
    #pragma warning restore 0618
}
     