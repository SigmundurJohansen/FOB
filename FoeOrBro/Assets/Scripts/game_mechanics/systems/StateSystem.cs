using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class StateSystem : ComponentSystem
{
    float checkStateTimer = 2;
    float inIdleTimer = 2;
    protected override void OnUpdate()
    {
        checkStateTimer -= UnityEngine.Time.deltaTime;
        if (checkStateTimer <= 0)
        {
            Entities.ForEach((Entity entity, ref OrderComponent _orders, ref StateComponent _state, ref HealthComponent _health, ref IDComponent _id, ref DeathComponent _death) =>
            {
                if (!_orders.hasOrders)
                {
                    int myID = _id.id;
                    int myState = 3;
                    // CHECK IF UNDER ATTACK
                    Entities.ForEach((Entity _entity, ref HasTarget _target) =>
                    {
                        IDComponent targetID = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<IDComponent>(_target.targetEntity);
                        if (myID == targetID.id)
                            myState = 1;
                    });
                    if (myState == 1)
                    {
                        _state.state = 1;
                        Debug.Log("Under attack!");
                    }
                    if (_state.state == 0)
                    {
                        //Debug.Log(_id.id + " is idle");
                        _state.state = 3;
                        PostUpdateCommands.AddComponent(entity, new RoamingComponent { });
                    }
                    if (_state.state == 1)
                    {
                        //Debug.Log(_id.id + " is in combat");

                        Entities.ForEach((Entity moraleEntity, ref StateComponent _moraleState, ref HealthComponent _moraleHealth, ref IDComponent _moraleId, ref MoraleComponent _morale) =>
                        {
                            if (_morale.healthModifier < 1)
                            {
                                _moraleState.state = 4;
                                Debug.Log(_moraleId.id + " is panicking!!");
                            }
                            if (_moraleHealth.health / _moraleHealth.maxHealth <= 0.8)
                                _morale.healthModifier = -1;
                            else if (_moraleHealth.health / _moraleHealth.maxHealth <= 0.5)
                                _morale.healthModifier = -2;
                            else
                                _morale.healthModifier = -4;
                        });
                    }
                    if (_state.state == 2)
                    {
                        //Debug.Log(_id.id + " is doing task");
                    }
                    if (_state.state == 3)
                    {
                        //Debug.Log(_id.id + " is roaming");
                    }
                    if (_state.state == 4)
                    {
                        Debug.Log(_id.id + " is Fleeing");
                    }
                }
                else
                {
                    _state.state = 2;
                }
            });
            checkStateTimer = 1;
        }
    }
}