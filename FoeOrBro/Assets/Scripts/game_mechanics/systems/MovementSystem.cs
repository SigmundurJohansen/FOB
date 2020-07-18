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
   [BurstCompile]
   struct MovementSystemJob : IJobForEach<Translation, MovementComponent>
   {
       public float DeltaTime;
       
       public void Execute(ref Translation _translation, ref MovementComponent _movement)
       {    
            if(_movement.isMoving){
               if( Mathf.Abs(_movement.destination.x - _translation.Value.x) < 2 || Mathf.Abs(_movement.destination.y - _translation.Value.y) < 2)
                    _movement.isMoving = false;
                var heading = _movement.destination - _translation.Value;
                var distance = heading.x* heading.x + heading.y*heading.y;
                var direction = heading / distance;
                _translation.Value += direction * _movement.speed *DeltaTime; 
           }else{
               if(Mathf.Abs(_movement.destination.x - _translation.Value.x) > 2 || Mathf.Abs(_movement.destination.y - _translation.Value.y) > 2)
                    _movement.isMoving = true;
           }
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
}

