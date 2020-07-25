using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct MovementComponent : IComponentData
{
    public bool isMoving;
    public float speed;
    public float3 direction;
    public float3 coords;
}
