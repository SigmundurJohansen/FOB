using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct AttackComponent  : IComponentData
{
    public bool isAttacking;
    public float nrOfAttacks;
    public float timer;
    public float range;
    public int weapon;
}
