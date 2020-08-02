
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
    [SerializeField]
    public Material terrainMaterial;
    
    private void Awake() {
        instance = this;
    }

    void Update(){
    }

    private void Start() { 

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SpawnGridMesh();
        SpawnPlayer();
        SpawnHumans(100);
    }

    public void SpawnGridMesh()
    {
        int width = PathfindingGridSetup.Instance.pathfindingGrid.GetWidth();
        int height = PathfindingGridSetup.Instance.pathfindingGrid.GetHeight();
        int count =  width* height;
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(NodeComponent),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Collider),
            typeof(NonUniformScale)
        );

        entityManager.CreateEntity(entityArchetype, entities);

        float offset = 3.2f;
        int entityCounter = 0;
        float cellSize = 0.32f;
        for (int y = 0; y < width; y++)
        {
            for(int x = 0; x < height; x++)
            {
                GridNode gridNode = (GridNode)PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y);
                Entity entity = entities[entityCounter];
                float3 myPosition = new float3(x*cellSize,y*cellSize, 10.5f);
                entityManager.SetComponentData(entities[entityCounter], new Translation {Value = myPosition});
                entityManager.SetSharedComponentData(entities[entityCounter], new RenderMesh { mesh = gridNode.GetNodeMesh(), material = gridNode.GetNodeMaterial() });
                if(!gridNode.IsWalkable()){
                    entityManager.SetComponentData(entities[entityCounter], new Collider { size = 0.32f });
                    entityManager.SetComponentData(entities[entityCounter], new NodeComponent { nodePosition = new int2(x,y), isWalkable = false});
                }else{                    
                    entityManager.SetComponentData(entities[entityCounter], new NodeComponent { nodePosition = new int2(x,y), isWalkable = true});
                }
                entityManager.SetComponentData(entities[entityCounter], new NonUniformScale { Value = 0.32f });
                entityCounter++;
            }
        }
        entities.Dispose();
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
        NativeArray<Entity> entities = new NativeArray<Entity>(1, Allocator.Temp);
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Player),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(MovementComponent),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(Collider),
            typeof(PlayerInputComponent),
            typeof(PathPosition),
            typeof(PathFollow),
            typeof(Selected),
            typeof(DestinationComponent),
            typeof(NonUniformScale)
        );
        entityManager.CreateEntity(entityArchetype, entities);
    
        Entity entity = entities[0];
        float3 myPosition = new float3(0, 0, -0.1f);
        float3 myDestination = new float3(0, 0, -0.1f);
        entityManager.SetComponentData(entities[0], new Translation {Value = myPosition});
        entityManager.SetComponentData(entities[0], new MovementComponent { isMoving = false, speed = 1.2f});
        entityManager.SetSharedComponentData(entities[0], new RenderMesh { mesh = spriteMesh, material = spriteMaterial });
        entityManager.SetComponentData(entities[0], new PlayerInputComponent { speed = 1});
        entityManager.SetComponentData(entities[0], new NonUniformScale { Value = 0.32f });
        entityManager.SetComponentData(entities[0], new Selected {isSelected = true});
        
        entities.Dispose();
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
            typeof(DestinationComponent),
            typeof(Collider),
            typeof(Selected),
            typeof(PathPosition),
            typeof(PathFollow),
            typeof(NonUniformScale)
        );

        entityManager.CreateEntity(entityArchetype, entities);

        for (int i = 0; i < count; i++)
        {            
            Entity entity = entities[i];
            float3 myPosition = new float3(UnityEngine.Random.Range(0, 10f), UnityEngine.Random.Range(0f, 10f), -0.1f);
            float3 myDestination = new float3(UnityEngine.Random.Range(0f, 20f), UnityEngine.Random.Range(0f, 20f), -0.1f);
            entityManager.SetComponentData(entities[i], new Translation {Value = myPosition});
            entityManager.SetComponentData(entities[i], new DestinationComponent {startPosition = new int2(8,8), endPosition = new int2(4,4)});
            entityManager.SetComponentData(entities[i], new MovementComponent { isMoving = true, speed = 1.0f});
            entityManager.SetSharedComponentData(entities[i], new RenderMesh { mesh = spriteMesh, material = spriteMaterial });
            entityManager.SetComponentData(entities[i], new PathFollow { pathIndex = 1});
            entityManager.SetComponentData(entities[i], new Collider { size = 0.16f });
            entityManager.SetComponentData(entities[i], new NonUniformScale { Value = 0.32f });
            entityManager.SetComponentData(entities[i], new Selected {isSelected = false});
        }
        entities.Dispose();
    }

    public struct Human : IComponentData { } 
    public struct Player : IComponentData { } 
}