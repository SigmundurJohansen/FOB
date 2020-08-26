using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
//using Unity.Jobs;

public class DamageSystem : ComponentSystem
{    
    protected override void OnUpdate() 
    {
        Entities.ForEach((Entity entity, ref HealthComponent _health, ref IDComponent _id) => {
            if(_health.health != GameController.Instance.GetUnitHealth(_id.id))
            {
                _health.health = GameController.Instance.GetUnitHealth(_id.id);
                Debug.Log("entity has taken damage, id "+_id.id);
            }
        });
       
    }
}