using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct DestinationComponent : IComponentData
{
    public int2 startPosition;
    public int2 endPosition;
}
