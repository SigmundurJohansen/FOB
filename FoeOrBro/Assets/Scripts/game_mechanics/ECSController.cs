
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
using UnityEngine.Rendering;


public class ECSController : MonoBehaviour {

    public static ECSController instance;
    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;
    private EntityManager entityManager;
    public Sprite mainSprite;

    public GameObject Prefab;
    [SerializeField]
    public Mesh spriteMesh;
    [SerializeField]
    public Material spriteMaterial;
    
    private void Awake() {
        instance = this;
    }

    void Update(){
    }

    private void Start() { 

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //SpawnUnits(10);
        SpawnHumans(2000);
    }

    public void SpawnPrefabs(int _count){
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);        
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);

        for (var x = 0; x < _count; x++)
        {
            var instance = entityManager.Instantiate(prefab);
            var position = transform.TransformPoint(new float3(1.3F, 2F, -0.3F));
            entityManager.SetComponentData(instance, new Translation {Value = position});        
        }
    }
    private void SpawnPlayer(){
        Entity entities = new Entity();
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Player),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(MovementComponent),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Collider),
            typeof(NonUniformScale)
        );

    }


    private void SpawnHumans(int count) {
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Human),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(MovementComponent),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Collider),
            typeof(NonUniformScale)
        );

        entityManager.CreateEntity(entityArchetype, entities);

        for (int i = 0; i < count; i++)
        {            
            Entity entity = entities[i];
            float3 myPosition = new float3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), -0.1f);
            float3 myDestination = new float3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), -0.1f);
            entityManager.SetComponentData(entities[i], new Translation {Value = myPosition});
            entityManager.SetComponentData(entities[i], new MovementComponent { isMoving = true, speed = 51.0f, destination = myDestination});
            entityManager.SetSharedComponentData(entities[i], new RenderMesh { mesh = spriteMesh, material = spriteMaterial });
            //entityManager.SetSharedComponentData(entities[i], new Collider { size = new Vector2(0.16f,0.16f) });
            entityManager.SetComponentData(entities[i], new NonUniformScale { Value = 0.32f });
        }
        entities.Dispose();
    }

    public struct Human : IComponentData { } 
    public struct Player : IComponentData { } 
}
 
public struct Collider : IComponentData
{
    public float Size;
}