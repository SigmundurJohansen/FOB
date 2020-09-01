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
using TMPro;
using SF = UnityEngine.SerializeField;

public class ECSController : MonoBehaviour
{
    public EntityManager entityManager;
    public static ECSController instance;
    public static ECSController Instance { get { return instance; } }
    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;
    public Sprite mainSprite;
    public GameObject koboltPrefab;
    public GameObject dragonPrefab;
    public GameObject playerPrefab;
    public GameObject listViewPrefab;
    public GameObject listViewParent;
    public BlobAssetStore blobAssetStore;
    [SF] public Mesh spriteMesh;
    [SF] public Material spriteMaterial;
    [SF] public Material terrainMaterial;

    EntityArchetype ArchKobolt;
    EntityArchetype ArchHuman;

    //private var ArchKobolt;
    //private var ArchHuman;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        LevelLoader.Instance.CreateMap();
        //SpawnPlayerPrefab();
    }

    public void CreateEntities(int count)
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        var myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(koboltPrefab, settings);

        for (int i = 0; i < count; i++)
        {
            var instance = entityManager.Instantiate(myPrefab);
            float xValueF = UnityEngine.Random.Range(0, 10f);
            float yValueF = UnityEngine.Random.Range(0f, 10f);
            int xValueI = (int)Mathf.Round(xValueF) * (int)Mathf.Round(PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize());
            int yValueI = (int)Mathf.Round(yValueF) * (int)Mathf.Round(PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize());
            entityManager.SetComponentData(instance, new Translation() { Value = new float3(new float3(xValueF, yValueF, -0.1f)) });
            entityManager.AddComponentData(instance, new IDComponent() { id = 1 });
            entityManager.AddComponentData(instance, new HealthComponent() { maxHealth = 100, health = 100 });
            entityManager.AddComponentData(instance, new MovementComponent() { isMoving = false, speed = 1.2f });
            entityManager.AddComponentData(instance, new PathFollow() { });
            entityManager.AddComponentData(instance, new Selected() { isSelected = false });
            entityManager.AddBuffer<PathPosition>(instance);
            PathPosition someBufferElement = new PathPosition();
            DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
            someBufferElement.position = new int2(xValueI, yValueI);
            someBuffer.Add(someBufferElement);
            someBufferElement.position = new int2(xValueI, yValueI);
            someBuffer.Add(someBufferElement);
        }
    }

    public int CreateEntity(string name)
    {
        float fcellSize = 0.32f;
        float xValueF = UnityEngine.Random.Range(0, 100f);
        float yValueF = UnityEngine.Random.Range(0f, 100f);
        int icellSize = (int)Mathf.Round(fcellSize);
        int xValueI = (int)Mathf.Round(xValueF);
        int yValueI = (int)Mathf.Round(yValueF);
        bool isWalkabler = PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(xValueI, yValueI).IsWalkable();
        if (!isWalkabler)
            return -1;
        xValueF = xValueF * fcellSize;
        yValueF = yValueF * fcellSize;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        Entity myPrefab;
        if (name == "kobolt")
            myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(koboltPrefab, settings);
        else
            myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(dragonPrefab, settings);

        var instance = entityManager.Instantiate(myPrefab);
        entityManager.SetComponentData(instance, new Translation() { Value = new float3(new float3(xValueF, yValueF, -0.1f)) });
        entityManager.AddComponentData(instance, new IDComponent() { id = GameController.Instance.GetID() });
        entityManager.AddComponentData(instance, new HealthComponent() { maxHealth = 100, health = 100 });
        entityManager.AddComponentData(instance, new MovementComponent() { isMoving = false, speed = 1.2f });
        entityManager.AddComponentData(instance, new PathFollow() { });
        entityManager.AddComponentData(instance, new Selected() { isSelected = false });
        entityManager.AddBuffer<PathPosition>(instance);
        PathPosition someBufferElement = new PathPosition();
        DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
        someBufferElement.position = new int2(xValueI, yValueI);
        someBuffer.Add(someBufferElement);
        someBufferElement.position = new int2(xValueI, yValueI);
        someBuffer.Add(someBufferElement);
        GameController.Instance.AddUnit(name, new Vector3(xValueF, yValueF, -0.1f));
        return 0;
    }

    public void DestroyUnit()
    {

    }

    public void SpawnPlayerPrefab()
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        var myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, settings);
        var instance = entityManager.Instantiate(myPrefab);
        entityManager.AddBuffer<PathPosition>(instance);
        DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
        PathPosition someBufferElement = new PathPosition();
        someBufferElement.position = new int2(0, 0);
        someBuffer.Add(someBufferElement);
        someBufferElement.position = new int2(0, 0);
        someBuffer.Add(someBufferElement);
        var spawnPosition = transform.TransformPoint(new float3(0, 0, -0.1f));
        entityManager.SetComponentData(instance, new Translation { Value = spawnPosition });
        entityManager.AddComponentData(instance, new Selected { isSelected = true });
        entityManager.AddComponentData(instance, new MovementComponent { isMoving = false, speed = 1.5f });
        entityManager.AddComponentData(instance, new PathFollow());
    }

    public void SpawnKobolt(int _count)
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        var myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(koboltPrefab, settings);

        for (int i = 0; i < _count; i++)
        {
            var instance = entityManager.Instantiate(myPrefab);
            entityManager.AddBuffer<PathPosition>(instance);
            DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
            PathPosition someBufferElement = new PathPosition();
            someBufferElement.position = new int2(1, 1);
            someBuffer.Add(someBufferElement);
            someBufferElement.position = new int2(1, 1);
            someBuffer.Add(someBufferElement);
            float xValueF = UnityEngine.Random.Range(0, 10f);
            float yValueF = UnityEngine.Random.Range(0f, 10f);
            var spawnPosition = transform.TransformPoint(new float3(xValueF, yValueF, -0.1f));
            entityManager.SetComponentData(instance, new Translation { Value = spawnPosition });
            entityManager.AddComponentData(instance, new Selected { isSelected = true });
            entityManager.AddComponentData(instance, new MovementComponent { isMoving = false, speed = 1.2f });
            entityManager.AddComponentData(instance, new PathFollow());
            entityManager.AddComponentData(instance, new IDComponent { id = GameController.Instance.GetID() });
            GameController.Instance.AddUnit("kobolt", new Vector3(xValueF, yValueF, -0.1f));
            //GameController.Instance.AddUnitName("Kobolt " + unitID);
        }
    }

    public void SpawnPrefabs(GameObject _prefab, float _x, float _y, bool _collidable)
    {
        int width = PathfindingGridSetup.Instance.pathfindingGrid.GetWidth();
        int height = PathfindingGridSetup.Instance.pathfindingGrid.GetHeight();
        float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(_prefab, settings);

        var instance = entityManager.Instantiate(prefab);
        var position = transform.TransformPoint(new float3((_x + 0.5f) * cellSize, (_y + 0.5f) * cellSize, 0.5f));
        entityManager.SetComponentData(instance, new Translation { Value = position });
    }

    public void SpawnGridMesh()
    {
        int width = PathfindingGridSetup.Instance.pathfindingGrid.GetWidth();
        int height = PathfindingGridSetup.Instance.pathfindingGrid.GetHeight();
        int count = width * height;
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(NodeComponent),
            typeof(RenderMesh),
            typeof(Translation),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(Collider),
            typeof(NonUniformScale),
            ComponentType.ReadWrite<WorldRenderBounds>(),
            ComponentType.ChunkComponent<ChunkWorldRenderBounds>()
        );
        entityManager.CreateEntity(entityArchetype, entities);
        //float offset = 3.2f;
        int entityCounter = 0;
        float cellSize = 0.32f;
        float3 MinBound = new float3(0, 0, 0);
        float3 MaxBound = new float3(50, 50, 50);
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                GridNode gridNode = (GridNode)PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y);
                Entity entity = entities[entityCounter];
                float3 myPosition = new float3(x * cellSize, y * cellSize, 1.5f);
                entityManager.SetComponentData(entities[entityCounter], new Translation { Value = myPosition });
                entityManager.SetSharedComponentData(entities[entityCounter], new RenderMesh { mesh = gridNode.GetNodeMesh(), material = gridNode.GetNodeMaterial() });
                if (!gridNode.IsWalkable())
                {
                    entityManager.SetComponentData(entities[entityCounter], new Collider { size = 0.32f });
                    entityManager.SetComponentData(entities[entityCounter], new NodeComponent { nodePosition = new int2(x, y), isWalkable = false });
                }
                else
                {
                    entityManager.SetComponentData(entities[entityCounter], new NodeComponent { nodePosition = new int2(x, y), isWalkable = true });
                }
                entityManager.SetComponentData(entities[entityCounter], new NonUniformScale { Value = 0.32f });
                //entityManager.SetComponentData(entities[entityCounter], new WorldRenderBounds { Value = MaxBound });
                entityCounter++;
            }
        }
        entities.Dispose();
    }


    private void SpawnPlayer()
    {
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
            typeof(Rotation),
            typeof(Selected),
            typeof(DestinationComponent),
            typeof(NonUniformScale)
        );
        entityManager.CreateEntity(entityArchetype, entities);

        Entity entity = entities[0];
        float3 myPosition = new float3(0, 0, -0.1f);
        float3 myDestination = new float3(0, 0, -0.1f);
        entityManager.SetComponentData(entities[0], new Translation { Value = myPosition });
        entityManager.SetComponentData(entities[0], new MovementComponent { isMoving = false, speed = 1.2f });
        entityManager.SetSharedComponentData(entities[0], new RenderMesh { mesh = spriteMesh, material = spriteMaterial });
        entityManager.SetComponentData(entities[0], new PlayerInputComponent { speed = 1 });
        entityManager.SetComponentData(entities[0], new NonUniformScale { Value = 0.32f });
        entityManager.SetComponentData(entities[0], new Selected { isSelected = true });
        int unitID = GameController.Instance.GetUnitListLength();
        entityManager.AddComponentData(entities[0], new IDComponent { id = unitID });
        GameController.Instance.AddUnit("Player", new Vector3(0, 0, -0.1f));
        entities.Dispose();
    }
    private void OnDestroy()
    {
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
        }
    }

    private void SpawnHumans(int count)
    {
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
            entityManager.SetComponentData(entities[i], new Translation { Value = myPosition });
            entityManager.SetComponentData(entities[i], new DestinationComponent { startPosition = new int2(8, 8), endPosition = new int2(4, 4) });
            entityManager.SetComponentData(entities[i], new MovementComponent { isMoving = true, speed = 1.0f });
            entityManager.SetSharedComponentData(entities[i], new RenderMesh { mesh = spriteMesh, material = spriteMaterial });
            entityManager.SetComponentData(entities[i], new PathFollow { pathIndex = 1 });
            entityManager.SetComponentData(entities[i], new Collider { size = 0.16f });
            entityManager.SetComponentData(entities[i], new NonUniformScale { Value = 0.32f });
            entityManager.SetComponentData(entities[i], new Selected { isSelected = false });
        }
        entities.Dispose();
    }

    public struct Human : IComponentData { }
    public struct Player : IComponentData { }
    public struct Kobolt : IComponentData { }
}