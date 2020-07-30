using Unity.Entities;
using Unity.Mathematics;

public struct RigidBody : IComponentData
{
    public float2 velocity;
    public float2 position;
}