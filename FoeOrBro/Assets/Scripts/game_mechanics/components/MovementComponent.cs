using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct MovementComponent : IComponentData
{
    public float speed;
    public bool isMoving;
    public bool chaseTarget;
}
