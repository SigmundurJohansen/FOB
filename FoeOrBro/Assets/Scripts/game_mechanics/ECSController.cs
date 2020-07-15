
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Threading;

public class ECSController : MonoBehaviour {

    public static ECSController instance;

    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;

    private EntityManager entityManager;
    public Sprite mainSprite;
    public SpriteRenderer mainRenderer;

    
    private void Awake() {
        instance = this;
    }

    void Update(){
    }

    private void Start() {
        //entityManager = World.Active.EntityManager;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SpawnGoblin();
        //unitSelectedCircleMesh = ECS_Animation.CreateMesh(8f, 5f);
    }

    private void SpawnGoblin(){
        SpawnGoblin(new float3(UnityEngine.Random.Range(-70f, 70f), UnityEngine.Random.Range(-60f, 60f), 0f));
    }


    private void SpawnGoblin(float3 spawnPosition) {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Goblin),
            typeof(Translation),
            typeof(MovementComponent)
        );

        Entity entity = entityManager.CreateEntity(entityArchetype);
        float3 hvaere = new float3(UnityEngine.Random.Range(-70f, 70f), UnityEngine.Random.Range(-60f, 60f), 0f);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
        entityManager.SetComponentData(entity, new MovementComponent { speed = 3.0f, position = spawnPosition , destination = hvaere} );
        //entityManager.SetComponentData(entity, new RendererComponent {  sprite =  mainSprite});
            
    }

    public struct Goblin : IComponentData { } 
}


