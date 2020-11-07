using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class DebugSystem : ComponentSystem
{
    bool debug = true;
    protected override void OnUpdate()
    {
        if (debug)
        {
            Entities.ForEach((Entity entity, ref Translation translation, ref HasTarget hasTarget) =>
            {
                if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(hasTarget.targetEntity))
                {
                    Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);
                    Debug.DrawLine(translation.Value, targetTranslation.Value);
                }
            });
        }
    }

}