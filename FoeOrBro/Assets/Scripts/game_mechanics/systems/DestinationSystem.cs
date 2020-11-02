
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.SceneManagement;

public class DestinationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<DestinationComponent>().ForEach((Entity entity, ref Translation translation, ref HasTarget hasTarget,ref MovementComponent _move) =>
        {
            if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(hasTarget.targetEntity))
            {
                Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);
                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out int endX, out int endY);
                PathfindingGridSetup.Instance.pathfindingGrid.GetXY(targetTranslation.Value, out int startX, out int startY);
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(entity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                _move.isMoving = true;
                Debug.Log("destination given");
            }
        });
    }

}
