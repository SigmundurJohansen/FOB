
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
/*
public class UnitMoveToTargetSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entities.ForEach((Entity unitEntity, ref HasTarget hasTarget, ref Translation translation, ref IDComponent _id) =>
        {
            if (entityManager.Exists(hasTarget.targetEntity))
            {
                Translation targetTranslation = entityManager.GetComponentData<Translation>(hasTarget.targetEntity);
                IDComponent targetID = entityManager.GetComponentData<IDComponent>(hasTarget.targetEntity);

                if (math.distancesq(translation.Value, targetTranslation.Value) < .1f)
                {
                    // Close to target, destroy it
                    Debug.Log("damage target id: " + targetID.id);
                    GameController.Instance.DamageUnit(targetID.id, 20, 0);
                    //PostUpdateCommands.DestroyEntity(hasTarget.targetEntity);
                    //PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                }
                else
                {
                    float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
                    float moveSpeed = 1f;
                    translation.Value += targetDir * moveSpeed * deltaTime;
                }
            }
            else
            {
                // Target Entity already destroyed
                PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
            }
        });
    }
}
 */