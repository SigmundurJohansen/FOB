using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class StateSystem : ComponentSystem
{
    float checkStateTimer = 2;
    float inIdleTimer = 2;
    protected override void OnUpdate()
    {
        checkStateTimer -= UnityEngine.Time.deltaTime;
        Entities.ForEach((Entity entity, ref StateComponent _state, ref HealthComponent _health, ref IDComponent _id, ref DeathComponent _death) =>
        {
            if(_state.state == 0)
            {
                Debug.Log("State going roaming");
                _state.state = 3;
                PostUpdateCommands.AddComponent(entity, new RoamingComponent { });
            }
        });
    }
}