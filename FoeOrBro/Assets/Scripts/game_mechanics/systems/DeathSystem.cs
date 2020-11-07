using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class DeathSystem : ComponentSystem
{
    float checkDeadTimer = 2;
    protected override void OnUpdate()
    {
        checkDeadTimer -= UnityEngine.Time.deltaTime;
        Entities.ForEach((Entity entity, ref HealthComponent _health, ref IDComponent _id, ref DeathComponent _death) =>
        {
            if (_health.health <= 0 && _death.isDead == false)
            {
                _death.isDead = true;
                Debug.Log("dead true " + _id.id);
                GameController.Instance.RemoveUnit(_id.id);
            }
            if (_death.isDead == true)
            {
                _death.corpseTimer -= UnityEngine.Time.deltaTime;
                if (_death.corpseTimer < 0)
                {
                    PostUpdateCommands.DestroyEntity(entity);
                }
            }
        });
    }
}