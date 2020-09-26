using System;
using Unity.Entities;

/// <summary>
/// An entity can be targetted.
/// </summary>
[Serializable]
public struct TargetableComponent : IComponentData
{
    byte dummy;
}