using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public enum damageType{
        Physical = 0,
        Fire,
        Ice,
        Magical
    }

    public void DestroyUnit()
    {
        Debug.Log("not implemented atm");
        //GameController.Instance.DamageUnit(2, 20, 0);
    }

    public void SpawnUnit()
    {
        ECSController.Instance.CreateEntity("dragon", false);
        ECSController.Instance.CreateEntity("kobolt", false);
    }
}
