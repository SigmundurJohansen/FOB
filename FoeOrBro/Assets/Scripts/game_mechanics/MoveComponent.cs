using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct MovementComponent : IComponentData
{
    public float speed;
    public float3 position;
    public float3 destination;
}
