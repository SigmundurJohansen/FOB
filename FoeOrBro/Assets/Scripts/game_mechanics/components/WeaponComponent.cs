using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct WeaponComponent  : IComponentData
{
    public int weapon;
    public int toHit;
    public int damage;
}
