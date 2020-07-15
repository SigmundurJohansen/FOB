using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using System.Threading;


public class MoveSystem {

    struct MovementJob {
       public float deltaTime;

       public void Execute(ref Translation _pos, ref MovementComponent _move)
        {
            float3 value = _pos.Value;
            value += _move.destination * _move.speed;
            
        }
    }

}
 

 /*
    Entities.WithAll<MovementComponent>().ForEach((ref Position _pos, ref MovementComponent _move) => {
        _pos = _pos.translation(_move.destination,_move.speed);
    });



    Entities.ForEach((ref Translation translation, ref moveSpeedComponent) =>{
    });
*/

/*
float3 position = translation.Value + new float3(0, -3f, +5f);
 */