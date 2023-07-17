using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem gridSystemScript;
    public GridLayout gridLayout;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    public Tilemap subTileMap;

    public Building buildingScript = null;
    private BoundsInt prevArea;
    public Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private GameObject target;
    private RaycastHit2D mHit;
    Camera mainCam = null;

    private void Awake()
    {
        gridSystemScript = this;
        string tilePath = @"Tiles/";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Remove();
        MouseClickAndInstall();
    }
    private void MouseClickAndInstall()
    {
        if (Input.GetMouseButtonDown(0))
        {
            target = GetClickedObject();
            if (target == null) return;

            if (target.CompareTag("Building"))
            {
                Debug.Log("타켓 찾음");
                buildingScript = target.GetComponent<Building>();
                buildingScript.BuildingData.isPlaced = false;
                if (GameManager.GetGameManager().isBuildingUIOn) return;
                ReSetTile();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (target != null && Installable())
            {
                Debug.Log("설치됨");
                Install();
            }
            target = null;
        }
    }
    private GameObject GetClickedObject()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mHit = Physics2D.Raycast(worldPoint, Vector2.zero, 10.0f);

        if (mHit.collider != null)
        {
            target = mHit.collider.gameObject;
        }
       // Debug.Log(target);
        return target;
    }

    private void Remove()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveArea(buildingScript.BuildingData.area);
            Destroy(buildingScript.gameObject);
        }
    }
   
    // 건물 관련 함수
    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesArea(area, mainTilemap);
        foreach (var b in baseArray)
        {
            if (b != tileBases[TileType.White])
            {
                Debug.Log("Cannot place here");
                return false;
            }
        }
        return true;
    }

    public void InstallArea(BoundsInt area)
    {
        ChangeTiles(area, TileType.Empty, subTileMap);
        ChangeTiles(area, TileType.Green, mainTilemap);
    }

    public void RemoveArea(BoundsInt area)
    {
        ChangeTiles(area, TileType.White, mainTilemap);
    }

    public void InitializeBuliding(Building building)
    {
        buildingScript = building;
        ShowBuildingArea();
    }

    public bool Installable()
    {
        Vector3Int positionInt = gridLayout.LocalToCell(buildingScript.transform.position);
        BoundsInt areaTemp = buildingScript.BuildingData.area;
        areaTemp.position = positionInt;

        if (CanTakeArea(areaTemp))
        {
            return true;
        }

        return false;
    }

    public void Install()
    {
        Vector3Int positionInt = gridLayout.LocalToCell(buildingScript.transform.position);
        Debug.Log(positionInt);
        BoundsInt areaTemp = buildingScript.BuildingData.area;
        areaTemp.position = positionInt;
        buildingScript.buildingData.isPlaced = true;
        InstallArea(areaTemp);
    }

    // 타일 관련 함수
    private void ClearSubTileMap()
    {
        TileBase[] prevAreaInSubTileMap = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(prevAreaInSubTileMap, TileType.Empty);
        subTileMap.SetTilesBlock(prevArea, prevAreaInSubTileMap);
    }

    public void ShowBuildingArea()
    {
        ClearSubTileMap();
        buildingScript.BuildingData.area.position = gridLayout.WorldToCell(buildingScript.gameObject.transform.position); // 건물위치저장
        BoundsInt buildingArea = buildingScript.BuildingData.area;

        TileBase[] baseArray = GetTilesArea(buildingArea, mainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }
        subTileMap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public TileBase[] GetTilesArea(BoundsInt area, Tilemap ttilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = ttilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    public void ChangeTiles(BoundsInt area, TileType type, Tilemap ttilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        ttilemap.SetTilesBlock(area, tileArray);
    }

    public void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    public void ReSetTile()
    {
        BoundsInt areaTemp = buildingScript.BuildingData.area;
        TileBase[] baseArray = GetTilesArea(areaTemp, subTileMap);
        int size = baseArray.Length;
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.Empty])
            {
                RemoveArea(areaTemp);
            }
        }
    }

    public void SaveTileMap()
    {
        BoundsInt bounds = mainTilemap.cellBounds;

        MapData mapData = new MapData();

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                TileBase temp = mainTilemap.GetTile(new Vector3Int(x, y, 0));

                if (temp != null)
                {
                    mapData.tiles.Add(temp);
                    mapData.tilePoses.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        string json = JsonUtility.ToJson(mapData);
        File.WriteAllText(Application.dataPath + "/testMap.json", json);
    }

    public void LoadMap()
    {
        string json = File.ReadAllText(Application.dataPath + "/testMap.json");
        MapData data = JsonUtility.FromJson<MapData>(json);

        mainTilemap.ClearAllTiles();

        for (int i = 0; i < data.tilePoses.Count; i++)
        {
            mainTilemap.SetTile(data.tilePoses[i], data.tiles[i]);
        }
    }
}

public class MapData // 저장용
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> tilePoses = new List<Vector3Int>();
}
