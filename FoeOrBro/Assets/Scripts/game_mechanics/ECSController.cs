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
    public bool useSectorSystem;
    public EntityManager entityManager;
    public static ECSController instance;
    public static ECSController Instance { get { return instance; } }
    public Transform selectionAreaTransform;
    //public Material unitSelectedCircleMaterial;
    //public Mesh unitSelectedCircleMesh;
    public Sprite mainSprite;
    public GameObject koboltPrefab;
    public GameObject dragonPrefab;
    public GameObject playerPrefab;
    public GameObject listViewPrefab;
    public GameObject listViewParent;
    public BlobAssetStore blobAssetStore;
    [SF] private Mesh quad;
    //[SF] public Mesh spriteMesh;
    //[SF] public Material spriteMaterial;
    //[SF] public Material terrainMaterial;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        LevelLoader.Instance.CreateMap();
        SpawnPlayerPrefab();
        CreateEntity("Dragon", new float2(16f, 32f));
        CreateEntity("kobolt", new float2(10f, 33f));
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

    public int CreateEntity(string name, float2 _location = new float2())
    {
        float entityHealth = 50;
        float fcellSize = 0.32f;
        float2 ValueF = new float2(0f, 0f);
        if (ValueF.Equals(_location))
            ValueF = SetRandomLocation();
        else
            ValueF = _location;
        int2 ValueI = ConvertFloat2(ValueF);
        bool isWalkabler = PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(ValueI.x, ValueI.y).IsWalkable();
        if (!isWalkabler)
        {
            Debug.Log("not walkable");
            return -1;
        }
        ValueF = ValueF * fcellSize;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        Entity myPrefab;
        if (name == "kobolt")
            myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(koboltPrefab, settings);
        else
            myPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(dragonPrefab, settings);

        var instance = entityManager.Instantiate(myPrefab);
        if (name == "kobolt")
        {
            entityManager.AddComponentData(instance, new RaceComponent() { race = 0 });
            entityManager.AddComponentData(instance, new Kobolt() { });
            entityManager.AddComponentData(instance, new SectorEntity { typeEnum = SectorEntity.TypeEnum.Unit });
            entityManager.AddComponentData(instance, new HealthComponent() { maxHealth = 30, health = 30 });
            entityManager.AddComponentData(instance, new AttackComponent() { isAttacking = false, nrOfAttacks = 1, timer = 1, range = 2, weapon = 0 });
            entityManager.AddComponentData(instance, new WeaponComponent() { weapon = 0, toHit = 0, damage = 5 });
            entityHealth = 30;
        }
        else
        {
            entityManager.AddComponentData(instance, new RaceComponent() { race = 1 });
            entityManager.AddComponentData(instance, new Dragon() { });
            entityManager.AddComponentData(instance, new SectorEntity { typeEnum = SectorEntity.TypeEnum.Target });
            entityManager.AddComponentData(instance, new HealthComponent() { maxHealth = 100, health = 100 });
            entityManager.AddComponentData(instance, new AttackComponent() { isAttacking = false, nrOfAttacks = 4, timer = 1, range = 2, weapon = 0 });
            entityManager.AddComponentData(instance, new WeaponComponent() { weapon = 0, toHit = 2, damage = 20 });
            entityHealth = 100;
        }

        entityManager.SetComponentData(instance, new Translation() { Value = new float3(new float3(ValueF.x, ValueF.y, -0.1f)) });
        entityManager.AddComponentData(instance, new IDComponent() { id = GameController.Instance.GetID() });
        entityManager.AddComponentData(instance, new MovementComponent() { isMoving = false, speed = 1.2f });
        entityManager.AddComponentData(instance, new PathFollow() { });
        entityManager.AddComponentData(instance, new Selected() { isSelected = false });
        entityManager.AddComponentData(instance, new FactionComponent() { });
        entityManager.AddComponentData(instance, new DeathComponent() { isDead = false, corpseTimer = 10.0f });
        entityManager.AddComponentData(instance, new TargetableComponent() { });
        entityManager.AddComponentData(instance, new TargetComponent() { });
        entityManager.AddBuffer<PathPosition>(instance);
        PathPosition someBufferElement = new PathPosition();
        DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
        someBufferElement.position = new int2(ValueI.x, ValueI.y);
        someBuffer.Add(someBufferElement);
        someBufferElement.position = new int2(ValueI.x, ValueI.y);
        someBuffer.Add(someBufferElement);
        //GameController.Instance.AddUnit(name, new Vector3(ValueF.x, ValueF.y, -0.1f), entityHealth);
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

        //float2 ValueF = SetRandomLocation();
        float2 ValueF = new float2(20f, 27f);
        int2 ValueI = ConvertFloat2(ValueF);

        float fcellSize = 0.32f;
        ValueF.x = ValueF.x * fcellSize;
        ValueF.y = ValueF.y * fcellSize;
        CameraController.Instance.SetCameraPositionS(new float3(ValueF.x, ValueF.y, -0.5f));
        entityManager.SetComponentData(instance, new Translation() { Value = new float3(new float3(ValueF.x, ValueF.y, -0.1f)) });
        entityManager.AddComponentData(instance, new IDComponent() { id = GameController.Instance.GetID() });
        entityManager.AddComponentData(instance, new HealthComponent() { maxHealth = 100, health = 100 });
        entityManager.AddComponentData(instance, new MovementComponent() { isMoving = false, speed = 1.5f });
        entityManager.AddComponentData(instance, new PathFollow() { });
        entityManager.AddComponentData(instance, new Player() { });
        entityManager.AddComponentData(instance, new AttackComponent() { isAttacking = false, nrOfAttacks = 2, timer = 1, range = 2, weapon = 0 });
        entityManager.AddComponentData(instance, new WeaponComponent() { weapon = 0, toHit = 0, damage = 10 });
        entityManager.AddComponentData(instance, new Selected() { isSelected = false });
        entityManager.AddBuffer<PathPosition>(instance);
        PathPosition someBufferElement = new PathPosition();
        DynamicBuffer<PathPosition> someBuffer = entityManager.GetBuffer<PathPosition>(instance);
        someBufferElement.position = new int2(ValueI.x, ValueI.y);
        someBuffer.Add(someBufferElement);
        someBufferElement.position = new int2(ValueI.x, ValueI.y);
        someBuffer.Add(someBufferElement);
        GameController.Instance.AddUnit("player", new Vector3(ValueF.x, ValueF.y, -0.1f), 100);
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
            entityManager.SetComponentData(instance, new Kobolt { });
            entityManager.SetComponentData(instance, new Translation { Value = spawnPosition });
            entityManager.AddComponentData(instance, new Selected { isSelected = true });
            entityManager.AddComponentData(instance, new MovementComponent { isMoving = false, speed = 1.2f });
            entityManager.AddComponentData(instance, new PathFollow());
            entityManager.AddComponentData(instance, new IDComponent { id = GameController.Instance.GetID() });
            GameController.Instance.AddUnit("kobolt", new Vector3(xValueF, yValueF, -0.1f), 30);
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


        Material cachMaterial = new Material(Shader.Find("Unlit/Texture"));
        Mesh cachmesh = quad;

        entityManager.SetSharedComponentData(entities[entityCounter], new RenderMesh { mesh = cachmesh, material = cachMaterial });


        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
                GridNode gridNode = (GridNode)PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y);
                Entity entity = entities[entityCounter];
                float3 myPosition = new float3(x * cellSize, y * cellSize, 1.5f);
                entityManager.SetComponentData(entities[entityCounter], new Translation { Value = myPosition });
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

    private void OnDestroy()
    {
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
        }
    }

    public int2 ConvertFloat2(float2 _value)
    {
        return new int2((int)Mathf.Round(_value.x), (int)Mathf.Round(_value.y));
    }

    public float2 RandomFloat2(float _min, float _max)
    {
        return new float2(UnityEngine.Random.Range(_min, _max), UnityEngine.Random.Range(_min, _max));
    }

    public int2 RandomInt2(float _min, float _max)
    {
        return new int2((int)Mathf.Round(UnityEngine.Random.Range(_min, _max)), (int)Mathf.Round(UnityEngine.Random.Range(_min, _max)));
    }

    public float2 SetRandomLocation()
    {
        float2 ValueF = RandomFloat2(1f, 99f);
        int2 ValueI = new int2((int)Mathf.Round(ValueF.x), (int)Mathf.Round(ValueF.y));
        bool isWalkabler = PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(ValueI.x, ValueI.y).IsWalkable();
        while (!isWalkabler)
        {
            int2 temp = RandomInt2(1f, 99f);
            isWalkabler = PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(temp.x, temp.y).IsWalkable();
        }
        return ValueF;
    }

}
public struct Human : IComponentData { }
public struct Player : IComponentData { }
public struct Kobolt : IComponentData { }
public struct Dragon : IComponentData { }
public enum Race
{
    kobolt = 0,
    dragon = 1,
    human = 2,
    player
};


public struct HasTarget : IComponentData
{
    public Entity targetEntity;
}

