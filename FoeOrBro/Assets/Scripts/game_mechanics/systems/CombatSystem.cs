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
        Entities.ForEach((Entity entity, ref Translation _translation, ref HasTarget _hasTarget, ref AttackComponent _attack, ref WeaponComponent _weapon) =>
        {
            if (_hasTarget.targetEntity != Entity.Null)
            {
                Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(_hasTarget.targetEntity);
                DeathComponent dead = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<DeathComponent>(_hasTarget.targetEntity);

                if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(_hasTarget.targetEntity) && _attack.range >= math.distance(_translation.Value, targetTranslation.Value) && _attack.isAttacking == true)
                {
                    _attack.timer -= UnityEngine.Time.deltaTime;
                    if (_attack.timer < 0)
                    {
                        IDComponent attackerID = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IDComponent>(entity);
                        Debug.Log("Attack!");
                        IDComponent targetID = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IDComponent>(_hasTarget.targetEntity);
                        GameController.Instance.DamageUnit(attackerID.id, targetID.id, _weapon.damage, GameController.damageType.Physical);
                        _attack.timer = 10 / _attack.nrOfAttacks;
                        if (dead.isDead)
                        {
                            Debug.Log("combat removing hastarget");
                            PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));
                        }
                    }
                }
            }
        });
    }
}
