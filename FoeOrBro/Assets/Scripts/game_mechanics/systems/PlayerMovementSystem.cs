
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using System.Threading;
using Unity.Burst;

public class PlayerMovementSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        /*
        Entities.ForEach((ref Translation _translation, ref PlayerInputComponent _player) => {
            var vert = Input.GetAxis("Horizontal") * _player.speed * _player.myDeltaTime;
            var horz = Input.GetAxis("Vertical") * _player.speed * _player.myDeltaTime;
            _translation.Value += new float3(vert, horz, 0);
        });
         */
    }
}

