using UnityEngine;
using System.Collections;
using Unity.Entities;

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
        
        cMap = SaveSystem.LoadMap();
        PathfindingGridSetup.Instance.CreateGrid(cMap.mapWidth, cMap.mapHeight);

        miniMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(cMap.mapWidth, cMap.mapHeight, cMap.mapTiles, 0.05f, 0.18f, 0.4f);
        //GameObject prefab = GetPrefabFromType(cMap.mapTiles[x, y].BiomeType);
        for (int y = 0; y < cMap.mapHeight; y++)
        {
            for (int x = 0; x < cMap.mapWidth; x++)
            {
                GameObject prefab = GetPrefabFromType(cMap.mapTiles[x, y].BiomeType);
                HeightType height = cMap.mapTiles[x, y].HeightType;
                
                if (height == HeightType.DeepWater)
                {
                    ECSController.Instance.SpawnPrefabs(prefabDeepWater, (float)x, (float)y, false);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(false);
                }
                if (height == HeightType.ShallowWater)
                {
                    ECSController.Instance.SpawnPrefabs(prefabWater, (float)x, (float)y, false);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(false);
                }
                bool collidable = cMap.mapTiles[x, y].Collidable;
                if (height != HeightType.DeepWater && height != HeightType.ShallowWater)
                {
                    ECSController.Instance.SpawnPrefabs(prefab, (float)x, (float)y, collidable);
                    PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(x, y).SetIsWalkable(collidable);
                }
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
                        pixels[x + y * _width] = Color.Lerp(pixels[x + y * _width], Color.black, 0.35f);
                }
            }
        }
        return GrassColor;
    }

    public GameObject GetPrefabFromType(BiomeType _type)
    {
        switch (_type)
        {
            case BiomeType.Desert:
                return prefabDesert;
            case BiomeType.Savanna:
                return prefabSavanna;
            case BiomeType.TropicalRainforest:
                return prefabTropicalRainforest;
            case BiomeType.Grassland:
                return prefabGrassland;
            case BiomeType.Woodland:
                return prefabWoodland;
            case BiomeType.SeasonalForest:
                return prefabSeasonalForest;
            case BiomeType.TemperateRainforest:
                return prefabTemperateRainforest;
            case BiomeType.BorealForest:
                return prefabBorealForest;
            case BiomeType.Tundra:
                return prefabTundra;
            case BiomeType.Ice:
                return prefabIce;
        }
        return prefabGrassland;
    }
}
