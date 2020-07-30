    /*
    using Unity.Entities;
    using Unity.Physics;
    using Unity.Physics.Systems;
     
    // Got basic structure of colliders from https://forum.unity.com/threads/ecs-collisions.857725/
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class System : SystemBase
    {
        EndFramePhysicsSystem EndFramePhysicsSystem => World.GetOrCreateSystem<EndFramePhysicsSystem>();
        StepPhysicsWorld StepPhysicsWorld => World.GetOrCreateSystem<StepPhysicsWorld>();
        BuildPhysicsWorld BuildPhysicsWorld => World.GetOrCreateSystem<BuildPhysicsWorld>();
     
        struct CollisionEventJob : ICollisionEventsJob
        {
            public void Execute(CollisionEvent evt)
            {
                UnityEngine.Debug.Log(evt);
            }
        }
     
        protected override void OnUpdate()
        {
            EndFramePhysicsSystem.GetOutputDependency().Complete();
            Dependency = new CollisionEventJob().Schedule
            (
                StepPhysicsWorld.Simulation,
                ref BuildPhysicsWorld.PhysicsWorld,
                Dependency
            );
        }
    }
 */