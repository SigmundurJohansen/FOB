using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public void DestroyUnit()
    {
        GameController.Instance.RemoveUnitName();
        GameController.Instance.OnGui();
    }

    public void SpawnUnit()
    {
        ECSController.Instance.CreateEntity("Dragon");
        ECSController.Instance.CreateEntity("kobolt");
        GameController.Instance.OnGui();
    }
}
