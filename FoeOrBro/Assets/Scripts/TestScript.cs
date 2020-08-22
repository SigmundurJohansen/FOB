using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public void DestroyUnit()
    {
        GameController.Instance.RemoveUnitName();
    }

    public void SpawnUnit()
    {
        int counter = GameController.Instance.GetUnitListLength();
        GameController.Instance.AddUnitName("Spawned Unit"+counter);
    }
}
