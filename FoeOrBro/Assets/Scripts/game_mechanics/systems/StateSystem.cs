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
        if(checkStateTimer <= 0)
        {
            Entities.ForEach((Entity entity, ref StateComponent _state, ref HealthComponent _health, ref IDComponent _id, ref DeathComponent _death) =>
            {
                if(_state.state == 0)
                {
                    Debug.Log(_id.id + " is idle");                
                    _state.state = 3;
                    PostUpdateCommands.AddComponent(entity, new RoamingComponent { });
                }
                if(_state.state == 1)
                {
                    Debug.Log(_id.id + " is in combat");
                }
                if(_state.state == 2)
                {
                    Debug.Log(_id.id + " is doing task");
                }
                if(_state.state == 3)
                {
                    Debug.Log(_id.id + " is roaming");
                }
            });
            checkStateTimer = 1;
        }
    }
}