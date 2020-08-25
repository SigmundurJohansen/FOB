using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class DeathSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        Entities.ForEach((Entity entity, ref HealthComponent _health, ref IDComponent _id) => {
            if(_health.health <=0)
            {
                PostUpdateCommands.DestroyEntity(entity);
                GameController.Instance.RemoveUnit(_id.id);
                Debug.Log("destroy entity nr" + _id.id);
            }
        });
       
    }

}