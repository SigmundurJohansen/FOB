using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct DamageComponent  : IComponentData
{
    public enum damageType{
        Physical = 0,
        Fire,
        Ice,
        Magical
    }
    public float amount;
}
