using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct NodeComponent : IComponentData
{
    public int2 nodePosition;
    public bool isWalkable;
}
