using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CombatSystem : ComponentSystem
{
    public static bool isDebug = false;
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref IDComponent _outerID, ref Translation _translation, ref HasTarget _hasTarget, ref AttackComponent _attack, ref WeaponComponent _weapon, ref StateComponent _state) =>
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
                        if (isDebug)
                            Debug.Log("Attack!");
                        IDComponent targetID = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IDComponent>(_hasTarget.targetEntity);
                        //GameController.Instance.DamageUnit(attackerID.id, targetID.id, _weapon.damage, GameController.damageType.Physical);
                        _attack.timer = 10 / _attack.nrOfAttacks;
                        float dmg = _weapon.damage;
                        int outerID = _outerID.id;
                        int newState = 1;
                        Entities.ForEach((Entity innerEntity, ref IDComponent _innerID, ref HealthComponent _health, ref DeathComponent _death) =>
                        {
                            if (targetID.id == _innerID.id)
                            {
                                if (isDebug)
                                    Debug.Log(outerID + " hits " + _innerID.id + " with " + dmg + " damage");
                                _health.health -= dmg;
                                if (isDebug)
                                    Debug.Log(_innerID.id + " has " + _health.health + "health left");
                                if (_health.health <= 0)
                                {
                                    _death.isDead = true;
                                    dead.isDead = true;
                                    newState = 0;
                                    if (isDebug)
                                        Debug.Log(_innerID.id + " has died in combat");
                                }
                            }
                        });
                        if (newState == 0)
                            _state.state = 0;

                        if (dead.isDead)
                        {
                            if (isDebug)
                                Debug.Log("combat removing hastarget");
                            Debug.Log("combat going idle");
                            PostUpdateCommands.AddComponent(entity, new IdleComponent { });
                            PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));
                        }
                    }
                }
            }
            else
            {
                _state.state = 0;
                Debug.Log("no target going idle");
            }
        });
    }
}
