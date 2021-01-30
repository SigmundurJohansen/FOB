using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
//using Unity.Jobs;

public class PlaceSystem : ComponentSystem
{
    float timer;
    protected override void OnUpdate()
    {
        timer -= UnityEngine.Time.deltaTime;
        Entities.ForEach((Entity entity, ref PlaceComponent _place, ref Translation _trans) =>
        {
            if (timer < 0)
            {
                if (!_place.isPlaced)
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -0.1f));
                    GetXY(mousePosition, out int endX, out int endY);
                    _trans.Value = new Vector3(mousePosition.x, mousePosition.y, -0.1f);
                    Debug.Log("placing");
                    timer = 0.1f;

                    if (GameController.Instance.GetSelectionSate() == 0)
                        _place.isPlaced = true;
                }
            }
        });
    }
    public void GetXY(Vector3 _vector3, out int _x, out int _y)
    {
        float x = _vector3.x;
        float y = _vector3.y;
        if (_vector3.x >= 100)
            x = 90;
        if (_vector3.x <= 0)
            x = 0;
        if (_vector3.y >= 100)
            y = 90;
        if (_vector3.y <= 0)
            y = 0;
        _x = Mathf.FloorToInt(x / 0.32f);
        _y = Mathf.FloorToInt(y / 0.32f);
    }
}