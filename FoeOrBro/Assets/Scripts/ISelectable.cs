using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISelectable{
    float currentHealth { get;set;}
    void Damage(float damageAmount);
    string Name();
}

