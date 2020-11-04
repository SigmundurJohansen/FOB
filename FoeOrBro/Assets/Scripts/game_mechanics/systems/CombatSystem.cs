
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

public class CombatSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation _translation, ref HasTarget hasTarget, ref MovementComponent _move, ref AttackComponent _attack, ref WeaponComponent _weapon) =>
        {
            float attackDelay = 10 / _attack.nrOfAttacks;
            Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);

            if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(hasTarget.targetEntity) && _attack.range >= math.distance(_translation.Value, targetTranslation.Value) && _attack.isAttacking == true)
            {
                _attack.timer -= UnityEngine.Time.deltaTime;
                if (_attack.timer < attackDelay)
                {
                    Debug.Log("Attack!");
                    HealthComponent targetHealth = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<HealthComponent>(hasTarget.targetEntity);
                    targetHealth.health = targetHealth.health - 20;
                    if (targetHealth.health <= 0)
                        PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));

                    _attack.timer = 10 / _attack.nrOfAttacks;
                }

            }

        });

    }

}
