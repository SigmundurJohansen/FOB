using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct HealthComponent  : IComponentData
{
    public float maxHealth;
    public float health;
}
