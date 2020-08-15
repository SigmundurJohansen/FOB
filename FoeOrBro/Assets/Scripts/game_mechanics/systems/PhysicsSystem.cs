
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class PhysicsSystem : ComponentSystem //JobComponentSystem
{
    #pragma warning disable 0618
    #pragma warning disable 0219
    private EntityQuery moving_group;
    private EntityQuery collider_group;

    protected override void OnUpdate() {

        List<PhysicsSystemJob> physicsSystemJobList = new List<PhysicsSystemJob>();
        NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
        
        Entities.ForEach((Entity _entity, ref RigidBody _body, ref Collider _colliders) => {

        });

        JobHandle.CompleteAll(jobHandleList);

        foreach (PhysicsSystemJob findCollisionJob in physicsSystemJobList) {
            new PhysicsSystemJob {
            }.Run();
        }

        jobHandleList.Dispose();
        
    }

    [BurstCompile]
    struct PhysicsSystemJob : IJob // IJobForEachWithEntity<RigidBody, Collider>
    {
        public float DeltaTime;
        [DeallocateOnJobCompletion]
        public Entity entity;
        public NativeArray<RigidBody> bodies;
        public NativeArray<Collider> colliders;
        public void Execute()// Entity entity, int index, ref RigidBody _body , ref Collider _colliders
        { 
            /* 
            NativeArray<RigidBody> bodyEntities = bodies;
            NativeArray<Collider> colliderEntities = colliders;
            foreach (var rigidBody in bodyEntities)
            {
                if (rigidBody.velocity.x != 0f || rigidBody.velocity.y != 0f)                           // Only update moving objects
                {
                    float2 displacement = rigidBody.velocity * DeltaTime;                                  // Compute number of steps and stepwise displacement
                    int steps = math.max(1, (int)math.round(math.length(displacement) / 0.05f));
                    float2 move_step = displacement / steps;

                    for (int s = 0; s < steps; s++)                                             // Update object position in substeps
                    {
                        rigidBody.position += move_step;                                            // Apply velocity (step-by-step)                    
                        bool collided = false;

                    #region hideme
                    
                        for (int j = 0; j < colliderEntities.Length; j++)                         // Iterate over all other colliders
                        {                      
                            collided = AreSquaresOverlapping(rigidBody.position, colliderEntities._colliders.size, colliderEntities[j]._body.position,  colliderEntities[j]._colliders.Collider.size);  // Check collision                                
                            if (collided)
                                break;
                        }
                    }
                    if (collided)// A collision occured
                    {
                        Debug.Log("collision detected");
                        rigidBody.velocity = new float2(0f, 0f);// Set velocity to 0
                        //moving_rigidbodies[i] = rigid_body;                        
                        break;// Do not perform any more substeps
                    }else{
                        //moving_positions[i] = position;// Store update position
                    }
                    
                    #endregion
                    
                }
            }
            */
        }
   }

    // Checks if the square at position posA and size sizeA overlaps 
    // with the square at position posB and size sizeB
    static bool AreSquaresOverlapping(float2 posA, float sizeA, float2 posB, float sizeB)
    {
        float d = (sizeA / 2) + (sizeB / 2);
        return (math.abs(posA.x - posB.x) < d && math.abs(posA.y - posB.y) < d);
    }

    static bool IsOutSideLevel(float2 pos, float size, int2 level_size)
    {
        float half_size = size / 2;
        return (pos.x - half_size < -0.5f
            || pos.y - half_size < -0.5f
            || pos.x + half_size > (float)level_size.x - 0.5f
            || pos.y + half_size > (float)level_size.y - 0.5f);
    }
}

         