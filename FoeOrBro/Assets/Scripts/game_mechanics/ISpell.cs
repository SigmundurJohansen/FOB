using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    GameObject gameObject { get; }
    int manaCost {get; set;}
    void Cast();
}
