using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct DeathComponent  : IComponentData
{
    public bool isDead;
    public float corpseTimer;
}
