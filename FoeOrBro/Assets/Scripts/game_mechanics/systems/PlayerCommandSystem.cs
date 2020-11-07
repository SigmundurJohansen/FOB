
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

public class PlayerComandSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation _translation, ref Player _player) =>
        {
            

        });

    }

}
