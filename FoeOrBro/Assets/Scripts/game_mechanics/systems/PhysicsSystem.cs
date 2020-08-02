
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

class PhysicsSystem : JobComponentSystem
{
    #pragma warning disable 0618
    #pragma warning disable 0219
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    private EntityQuery moving_group;
    private EntityQuery collider_group;

    protected override void OnCreate()
    {        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        //moving_group = EntityManager.CreateEntityQuery(typeof(RigidBody), typeof(Collider));
        //collider_group = EntityManager.CreateEntityQuery(typeof(RigidBody), typeof(Collider));
    }
    
    [BurstCompile]
    struct PhysicsSystemJob : IJobForEachWithEntity<RigidBody, Collider>
    {       
        public float DeltaTime;
        public void Execute(Entity entity, int index, ref RigidBody _body , ref Collider _c)
        { 
            if (_body.velocity.x != 0f || _body.velocity.y != 0f)                           // Only update moving objects
            {
                float2 displacement = _body.velocity * DeltaTime;                                  // Compute number of steps and stepwise displacement
                int steps = math.max(1, (int)math.round(math.length(displacement) / 0.05f));
                float2 move_step = displacement / steps;

                for (int s = 0; s < steps; s++)                                             // Update object position in substeps
                {
                    _body.position += move_step;                                            // Apply velocity (step-by-step)                    
                    bool collided = false;

                    #region hideme
                    /*
                        for (int j = 0; j < colliderEntities.Length; j++)                         // Iterate over all other colliders
                        {                      
                            collided = AreSquaresOverlapping(_body.position, _colliders.size, colliderEntities[j]._body.position,  colliderEntities[j]._colliders.Collider.size);  // Check collision                                
                            if (collided)
                                break;
                        }
                    }
                    if (collided)// A collision occured
                    {
                        _body.velocity = new float2(0f, 0f);// Set velocity to 0
                        moving_rigidbodies[i] = rigid_body;                        
                        break;// Do not perform any more substeps
                    }else{
                        moving_positions[i] = position;// Store update position
                    }
                    */
                    #endregion
                }                
            }
        }
   }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dt = UnityEngine.Time.deltaTime;
        
        //var movingEntities = moving_group.ToEntityArray(Allocator.TempJob);
        //var colliderEntities = collider_group.ToEntityArray(Allocator.TempJob);

        var job = new PhysicsSystemJob()
        {
            DeltaTime = UnityEngine.Time.deltaTime
        };
        return job.Schedule(this, inputDeps);

        //var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        /*
        return Entities.WithAll<RigidBody>().WithAll<Collider>()
                .ForEach((Entity entity, int nativeThreadIndex, ref RigidBody _body, ref Collider _colliders) =>{};
        */
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

    /*
 */