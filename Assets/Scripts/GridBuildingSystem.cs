using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem current;
    public GridLayout gridLayout;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private Tilemap subTileMap;

    public Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private BuildingSystem buildingScript = null;
    private BoundsInt prevArea;

    private GameObject target;

    private void Awake()
    {
        current = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        string tilePath = @"Tiles/";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
    }

    // Update is called once per frame
    void Update()
    {
/*        if (!buildingScript)
        {
            return;
        }*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ClearArea();
            ReTakeArea(buildingScript.Building.BuildingData.area);
            //Destroy(buildingScript.gameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            GetClickedObject();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (target != null && buildingScript.Installable())
            {
                buildingScript.Install();
                Debug.Log("설치됨");
            }
            target = null;
        }
    }

    // 타일 관련 함수
    public TileBase[] GetTilesBlock(BoundsInt area, Tilemap ttilemap)
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

    private void SetTileBlock(BoundsInt area, TileType type, Tilemap ttilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        ttilemap.SetTilesBlock(area, tileArray);
    }

    private void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    // 건물 관련 함수

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        subTileMap.SetTilesBlock(prevArea, toClear);
    }

    public void FollowBuilding()
    {
        ClearArea();
        buildingScript.Building.BuildingData.area.position = gridLayout.WorldToCell(buildingScript.gameObject.transform.position); // 건물위치저장
        BoundsInt buildingArea = buildingScript.Building.BuildingData.area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, mainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                Debug.Log("hi");
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

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, mainTilemap);
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

    public void TakeArea(BoundsInt area)
    {
        SetTileBlock(area, TileType.Empty, subTileMap);
        SetTileBlock(area, TileType.Green, mainTilemap);
    }

    public void ReTakeArea(BoundsInt area)
    {
        SetTileBlock(area, TileType.White, subTileMap);
        SetTileBlock(area, TileType.White, mainTilemap);
    }

    public void InitializeWithBuliding(BuildingSystem building)
    {
        //buildingScript = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<Building>();
        buildingScript = building;
        FollowBuilding();
    }

    private void GetClickedObject()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit;
        /*Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.collider.gameObject;
        }*/
        hit = Physics2D.Raycast(worldPoint, Vector2.zero, 10.0f);
        if (hit.collider == null)
            return;
        target = hit.collider.gameObject;
        if (target.CompareTag("Building"))
        {
            buildingScript = target.GetComponent<BuildingSystem>();
            buildingScript.Building.BuildingData.isPlaced = false;
            ReSetTile();
        }
    }

    public void ReSetTile()
    {
        BoundsInt areaTemp = buildingScript.Building.BuildingData.area;
        TileBase[] baseArray = GetTilesBlock(areaTemp, subTileMap);
        int size = baseArray.Length;
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.Empty])
            {
                ReTakeArea(areaTemp);
            }
        }
    }


}
