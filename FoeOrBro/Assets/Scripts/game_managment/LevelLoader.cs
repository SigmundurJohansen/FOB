using UnityEngine;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[System.Serializable]
public class ColorToPrefab
{
    public Color32 color;
    public GameObject prefab;
}

public enum BiomeType
{
    Desert = 0,
    Savanna = 1,
    TropicalRainforest = 2,
    Grassland = 3,
    Woodland = 4,
    SeasonalForest = 5,
    TemperateRainforest = 6,
    BorealForest = 7,
    Tundra = 8,
    Ice = 9
}

public class LevelLoader : MonoBehaviour
{
    static LevelLoader _instance;
    public static LevelLoader Instance { get { return _instance; } }
    public string levelFileName;
    int cWidth;
    int cHeight;
    Map cMap;

    //public BiomeType BiomeType;
    //public Texture2D levelMap;
    public BlobAssetStore blobAssetStore;
    public GameObject prefabDesert;
    public GameObject prefabSavanna;
    public GameObject prefabTropicalRainforest;
    public GameObject prefabGrassland;
    public GameObject prefabWoodland;
    public GameObject prefabSeasonalForest;
    public GameObject prefabTemperateRainforest;
    public GameObject prefabBorealForest;
    public GameObject prefabTundra;
    public GameObject prefabIce;
    public GameObject prefabWater;
    public GameObject prefabDeepWater;

    private static Color Ice = Color.white;
    private static Color Desert = new Color(238 / 255f, 218 / 255f, 130 / 255f, 1);
    private static Color Savanna = new Color(177 / 255f, 209 / 255f, 110 / 255f, 1);
    private static Color TropicalRainforest = new Color(66 / 255f, 123 / 255f, 25 / 255f, 1);
    private static Color Tundra = new Color(96 / 255f, 131 / 255f, 112 / 255f, 1);
    private static Color TemperateRainforest = new Color(29 / 255f, 73 / 255f, 40 / 255f, 1);
    private static Color Grassland = new Color(164 / 255f, 225 / 255f, 99 / 255f, 1);
    private static Color SeasonalForest = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1);
    private static Color BorealForest = new Color(95 / 255f, 115 / 255f, 62 / 255f, 1);
    private static Color Woodland = new Color(139 / 255f, 175 / 255f, 90 / 255f, 1);
    private static Color IceWater = new Color(210 / 255f, 255 / 255f, 252 / 255f, 1);
    private static Color ColdWater = new Color(119 / 255f, 156 / 255f, 213 / 255f, 1);
    private static Color RiverWater = new Color(65 / 255f, 110 / 255f, 179 / 255f, 1);
    private static Color DeepColor = new Color(15 / 255f, 30 / 255f, 80 / 255f, 1);
    private static Color ShallowColor = new Color(15 / 255f, 40 / 255f, 90 / 255f, 1);
    private static Color RiverColor = new Color(30 / 255f, 120 / 255f, 200 / 255f, 1);
    private static Color SandColor = new Color(198 / 255f, 190 / 255f, 31 / 255f, 1);
    private static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
    private static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
    private static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
    private static Color SnowColor = new Color(1, 1, 1, 1);

    public MeshRenderer miniMapRenderer;
    void Awake()
    {
        _instance = this;
        blobAssetStore = new BlobAssetStore();
    }

    void EmptyMap()
    {
        // Find all of our children and...eliminate them.
        while (transform.childCount > 0)
        {
            Transform c = transform.GetChild(0);
            c.SetParent(null); // become Batman
            Destroy(c.gameObject); // become The Joker
        }
    }

    void LoadAllLevelNames()
    {
        // Read the list of files from StreamingAssets/Levels/*.png
        // The player will progess through the levels alphabetically
    }


    public void CreateMap()
    {
        // LOAD MAP
        cMap = SaveSystem.LoadMap();
        // Create Grid
        PathfindingGridSetup.Instance.CreateGrid(cMap.mapWidth, cMap.mapHeight);
        float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
        Debug.Log("cellsize" + cellSize);
        // Mini Map
        miniMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(cMap.mapWidth, cMap.mapHeight, cMap.mapTiles, 0.05f, 0.18f, 0.4f);
        // Convert prefabs into entities
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        NativeArray<Entity> entities = new NativeArray<Entity>(13, Allocator.Temp);
        entities[0]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabDesert, settings);
        entities[1]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabSavanna, settings);
        entities[2]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabTropicalRainforest, settings);
        entities[3]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabGrassland, settings);
        entities[4]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabWoodland, settings);
        entities[5]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabSeasonalForest, settings);
        entities[6]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabTemperateRainforest, settings);
        entities[7]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabBorealForest, settings);
        entities[8]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabTundra, settings);
        entities[9]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabIce, settings);
        entities[10]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabWater, settings);
        entities[11]= GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabDeepWater, settings);

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int y = 0; y < cMap.mapHeight; y++)
        {
            for (int x = 0; x < cMap.mapWidth; x++)
            {
                var prefab = entities[GetPrefabFromType(cMap.mapTiles[x, y].BiomeType)];
                HeightType height = cMap.mapTiles[x, y].HeightType;
                Entity instance;
                bool collidable = cMap.mapTiles[x, y].Collidable;
                if (height == HeightType.DeepWater)
                {
                    instance = manager.Instantiate(entities[11]);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(false);
                }
                else if (height == HeightType.ShallowWater)
                {
                    instance = manager.Instantiate(entities[10]);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(false);
                }
                else //(height != HeightType.DeepWater && height != HeightType.ShallowWater)
                {
                    instance = manager.Instantiate(prefab);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(collidable);
                }

                var position = transform.TransformPoint(new float3(x+0.5f , y+0.5f  , 1f));
                manager.SetComponentData(instance, new Translation
                {
                    Value = position
                });
            }
        }

    }

    public Color GetTileColor(int _width, int _height, Tile[,] _tiles, float _coldest, float _colder, float _cold)
    {
        var texture = new Texture2D(_width, _height);
        var pixels = new Color[_width * _height];
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                BiomeType value = _tiles[x, y].BiomeType;
                pixels[x + y * _width] = Ice;
                // Water tiles
                if (_tiles[x, y].HeightType == HeightType.DeepWater)
                {
                    pixels[x + y * _width] = DeepColor;
                }
                else if (_tiles[x, y].HeightType == HeightType.ShallowWater)
                {
                    pixels[x + y * _width] = ShallowColor;
                }
                // draw rivers
                if (_tiles[x, y].HeightType == HeightType.River)
                {
                    float heatValue = _tiles[x, y].HeatValue;

                    if (_tiles[x, y].HeatType == HeatType.Coldest)
                        pixels[x + y * _width] = Color.Lerp(IceWater, ColdWater, (heatValue) / (_coldest));
                    else if (_tiles[x, y].HeatType == HeatType.Colder)
                        pixels[x + y * _width] = Color.Lerp(ColdWater, RiverWater, (heatValue - _coldest) / (_colder - _coldest));
                    else if (_tiles[x, y].HeatType == HeatType.Cold)
                        pixels[x + y * _width] = Color.Lerp(RiverWater, ShallowColor, (heatValue - _colder) / (_cold - _colder));
                    else
                        pixels[x + y * _width] = ShallowColor;
                }
                // add a outline
                if (_tiles[x, y].HeightType >= HeightType.Shore && _tiles[x, y].HeightType != HeightType.River)
                {
                    if (_tiles[x, y].BiomeBitmask != 15)
                        pixels[x + y * _width] = Color.Lerp(pixels[x + y * _width], Color.black, 0.32f);
                }
            }
        }
        return GrassColor;
    }

    public int GetPrefabFromType(BiomeType _type)
    {
        switch (_type)
        {
            case BiomeType.Desert:
                return 0;
            case BiomeType.Savanna:
                return 1;
            case BiomeType.TropicalRainforest:
                return 2;
            case BiomeType.Grassland:
                return 3;
            case BiomeType.Woodland:
                return 4;
            case BiomeType.SeasonalForest:
                return 5;
            case BiomeType.TemperateRainforest:
                return 6;
            case BiomeType.BorealForest:
                return 7;
            case BiomeType.Tundra:
                return 8;
            case BiomeType.Ice:
                return 9;
        }
        return 3;
    }
}
