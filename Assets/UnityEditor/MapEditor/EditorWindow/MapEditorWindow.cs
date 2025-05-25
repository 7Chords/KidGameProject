using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;

/**
 * 地图编辑器窗口脚本
 */
public class MapEditorWindow : EditorWindow
{
    public static MapEditorWindow Instance;

    private MapEditorConfig mapEditorConfig = new MapEditorConfig();

    public Color unSelectColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
    public Color selectColor = Color.yellow;

    [MenuItem("自定义编辑器/地图编辑器")]
    public static void ShowExample()
    {
        MapEditorWindow wnd = GetWindow<MapEditorWindow>();
        wnd.titleContent = new GUIContent("地图编辑器");
    }

    private VisualElement root;
    public void CreateGUI()
    {
        Instance = this;

        root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UnityEditor/MapEditor/EditorWindow/MapEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        InitTopMenu();
        InitItemMenu();
        InitWorkSpace();
    }



    #region TopMenu
    private Button EditMapButton;
    private Button InfoButton;
    private Button GenerateMapButton;


    private ObjectField MapDataField;
    private ObjectField ItemConfigField;//菜单栏配置数据

    private MapData mapData;
    private MapEditorItemGroupConfig mapEditorItemGroupConfig;

    private void InitTopMenu()
    {

        EditMapButton = root.Q<Button>(nameof(EditMapButton));
        EditMapButton.clicked += OnEditMapButtonClicked;

        InfoButton = root.Q<Button>(nameof(InfoButton));
        InfoButton.clicked += OnInfoButtonClicked;

        GenerateMapButton = root.Q<Button>(nameof(GenerateMapButton));
        GenerateMapButton.clicked += OnGenerateMapButtonClicked;

        MapDataField = root.Q<ObjectField>(nameof(MapDataField));
        MapDataField.objectType = typeof(MapData);
        MapDataField.RegisterValueChangedCallback(MapDataFieldValueChanged);
        MapDataField.RegisterCallback<MouseDownEvent>(OnMapDataFieldClicked);

        ItemConfigField = root.Q<ObjectField>(nameof(ItemConfigField));
        ItemConfigField.objectType = typeof(MapEditorItemGroupConfig);
        ItemConfigField.RegisterValueChangedCallback(ItemConfigFieldValueChanged);
        ItemConfigField.RegisterCallback<MouseDownEvent>(OnItemConfigFieldClicked);

        //默认是房间编辑模式
        OnEditMapButtonClicked();
    }

    private void OnEditMapButtonClicked()
    {
        SetButtonBorderColor(EditMapButton, selectColor);
        UpdateWorkSpaceView();
    }
    public void SetButtonBorderColor(Button btn, Color color)
    {
        btn.style.borderBottomColor = color;
        btn.style.borderTopColor = color;
        btn.style.borderLeftColor = color;
        btn.style.borderRightColor = color;
    }

    private void OnInfoButtonClicked()
    {
        Debug.Log("配置好菜单和地图数据后，可以选择左侧的按钮放置,家具必须放在地砖上\n"+
            "鼠标左键放置，中键拖拽工作区，滚轮放大缩小，右键删除");
    }
    private void OnGenerateMapButtonClicked()
    {
        if (mapData == null) return;
        GameObject root = new GameObject("Map");
        foreach(var tile in mapData.tileList)
        {
            GameObject tileGO = Instantiate(tile.tileData.tilePrefab,
                new Vector3(tile.mapPos.x,0, -tile.mapPos.y),
                Quaternion.identity);
            tileGO.transform.SetParent(root.transform);
        }
        foreach (var furniture in mapData.furnitureList)
        {
            float averageX = 0, averageZ = 0;
            foreach(var pos in furniture.mapPosList)
            {
                averageX += pos.x;
                averageZ += pos.y;
            }
            averageX /= furniture.mapPosList.Count;
            averageZ /= furniture.mapPosList.Count;
            GameObject furnitureGO = Instantiate(furniture.furnitureData.furniturePrefab);
            furnitureGO.transform.position = new Vector3(averageX, 0, -averageZ);
            furnitureGO.transform.SetParent(root.transform);
        }
    }

    private void MapDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        mapData = evt.newValue as MapData;
        UpdateWorkSpaceView();
    }
    private void OnMapDataFieldClicked(MouseDownEvent evt)
    {
        if (mapData != null)
        {
            Selection.activeObject = mapData;//设置Inspector显示
        }
    }
    private void ItemConfigFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        mapEditorItemGroupConfig = evt.newValue as MapEditorItemGroupConfig;
        SetItemMenu(ItemMenuType.Tile);
        RefreshItemGroupView();
    }
    private void OnItemConfigFieldClicked(MouseDownEvent evt)
    {
        if (mapEditorItemGroupConfig != null)
        {
            Selection.activeObject = mapEditorItemGroupConfig;
        }
    }
    #endregion


    #region ItemMenu
    private VisualElement ItemListView;
    private List<MapEditorItem> editorItemList;
    private Button LeftButton;
    private Button RightButton;
    private Label ItemGroupTitle;

    private MapEditorItem curSelectItem;

    private ItemMenuType itemMenuType;

    private void InitItemMenu()
    {
        ItemListView = root.Q<VisualElement>(nameof(ItemListView));
        LeftButton = root.Q<Button>(nameof(LeftButton));
        LeftButton.clicked += OnLeftButtonClicked;

        RightButton = root.Q<Button>(nameof(RightButton));
        RightButton.clicked += OnRightButtonClicked;

        ItemGroupTitle = root.Q <Label>(nameof(ItemGroupTitle));


        editorItemList = new List<MapEditorItem>();
    }

    private void OnLeftButtonClicked()
    {
        SetItemMenu(itemMenuType == ItemMenuType.Tile ? ItemMenuType.Furniture : ItemMenuType.Tile);
    }

    private void OnRightButtonClicked()
    {
        SetItemMenu(itemMenuType == ItemMenuType.Tile ? ItemMenuType.Furniture : ItemMenuType.Tile);
    }

    private void SetItemMenu(ItemMenuType menuType)
    {
        itemMenuType = menuType;
        switch (itemMenuType)
        {
            case ItemMenuType.Tile:
                ItemGroupTitle.text = "地砖";
                break;
            case ItemMenuType.Furniture:
                ItemGroupTitle.text = "家具";
                break;
            default:
                break;
        }
        curSelectItem = null;
        RefreshItemGroupView();
    }

    

    private void RefreshItemGroupView()
    {
        ClearItemGroupView();
        if (mapEditorItemGroupConfig == null) return;
        if (itemMenuType == ItemMenuType.Tile)
        {
            for (int i = 0; i < mapEditorItemGroupConfig.TileList.Count; i++)
            {
                MapEditorItem editItem = new MapEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView, mapEditorItemGroupConfig.TileList[i].tileName);
            }
        }
        else if(itemMenuType == ItemMenuType.Furniture)
        {
            for (int i = 0; i < mapEditorItemGroupConfig.FrunitureList.Count; i++)
            {
                MapEditorItem editItem = new MapEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView, mapEditorItemGroupConfig.FrunitureList[i].FurnitureName);
            }
        }
    }
    private void ClearItemGroupView()
    {
        foreach (var item in editorItemList)
        {
            item.Destory();
        }
        editorItemList.Clear();
    }

    public void SelectOneItem(MapEditorItem selectItem)
    {
        curSelectItem = selectItem;
        foreach (var item in editorItemList)
        {
            if (item == selectItem)
            {
                item.Select();
            }
            else
            {
                item.UnSelect();
            }
        }
    }
    #endregion

    #region WorkSpace
    private IMGUIContainer WorkContainer;

    private bool mouseCenterDrag;

    private float startOffsetX, startOffsetY;

    private void InitWorkSpace()
    {
        WorkContainer = root.Q<IMGUIContainer>(nameof(WorkContainer));
        //IMGUI的绘制函数
        WorkContainer.onGUIHandler = DrawWorkSpace;
        WorkContainer.RegisterCallback<WheelEvent>(WorkSpaceWheel);
        WorkContainer.RegisterCallback<MouseDownEvent>(WorkSpaceMouseDown);
        WorkContainer.RegisterCallback<MouseMoveEvent>(WorkSpaceMouseMove);
        WorkContainer.RegisterCallback<MouseUpEvent>(WorkSpaceMouseUp);
        WorkContainer.RegisterCallback<MouseOutEvent>(WorkSpaceMouseOut);

    }

    private void WorkSpaceWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        mapEditorConfig.curGridUnitLength = Mathf.Clamp(mapEditorConfig.curGridUnitLength- delta
            ,MapEditorConfig.minGridUnitLength,MapEditorConfig.maxGridUnitLength);
        UpdateWorkSpaceView();
        //Debug.Log(mapEditorConfig.curGridUnitLength);
    }
    private void UpdateWorkSpaceView()
    {
        if (WorkContainer == null) return;
        WorkContainer.MarkDirtyLayout(); // 标志为需要重新绘制的
    }
    private void DrawWorkSpace()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;

        //画网格
        Rect rect = WorkContainer.contentRect;

        float height = rect.height > MapEditorConfig.maxMapSizeY ? MapEditorConfig.maxMapSizeY : rect.height;
        float width = rect.width > MapEditorConfig.maxMapSizeX ? MapEditorConfig.maxMapSizeX : rect.width;

        //画横线
        float startY = startOffsetY % mapEditorConfig.curGridUnitLength;
        for (float i = mapEditorConfig.curGridUnitLength-startY; i<= height; i += mapEditorConfig.curGridUnitLength)
        {
            Handles.DrawLine(new Vector3(0, i),
                new Vector3(width, i));
        }

        //画竖线
        float startX = startOffsetX % mapEditorConfig.curGridUnitLength;
        for (float i = mapEditorConfig.curGridUnitLength-startX; i <= width; i += mapEditorConfig.curGridUnitLength)
        {
            Handles.DrawLine(new Vector3(i, 0),
                new Vector3(i, height));
        }

        //画数据
        if (mapData != null)
        {
            foreach (var tile in mapData.tileList)
            {
                float offsetGridX = startOffsetX / mapEditorConfig.curGridUnitLength;
                float offsetGridY = startOffsetY / mapEditorConfig.curGridUnitLength;
                if ((tile.mapPos.x - offsetGridX) < 0 || (tile.mapPos.y - offsetGridY) < 0) continue;
                GUI.DrawTexture(new Rect((tile.mapPos.x - offsetGridX) * mapEditorConfig.curGridUnitLength,
                    (tile.mapPos.y - offsetGridY) * mapEditorConfig.curGridUnitLength,
                    mapEditorConfig.curGridUnitLength,
                    mapEditorConfig.curGridUnitLength), tile.tileData.texture);

            }

            foreach(var furniture in mapData.furnitureList)
            {
                int mapXMin = 999, mapXMax = -1, mapYMin = 999, mapYMax = -1;
                foreach(var pos in furniture.mapPosList)
                {
                    if (pos.x < mapXMin) mapXMin = pos.x;
                    if (pos.x > mapXMax) mapXMax = pos.x;
                    if (pos.y < mapYMin) mapYMin = pos.y;
                    if (pos.y > mapYMax) mapYMax = pos.y;
                }
                float offsetGridX = startOffsetX / mapEditorConfig.curGridUnitLength;
                float offsetGridY = startOffsetY / mapEditorConfig.curGridUnitLength;
                if ((mapXMin - offsetGridX) < 0 || (mapYMin - offsetGridY) < 0) continue;
                GUI.DrawTexture(new Rect((mapXMin - offsetGridX) * mapEditorConfig.curGridUnitLength,
                    (mapYMin - offsetGridY) * mapEditorConfig.curGridUnitLength,
                    (mapXMax- mapXMin+1) * mapEditorConfig.curGridUnitLength,
                    (mapYMax - mapYMin + 1) * mapEditorConfig.curGridUnitLength), furniture.furnitureData.texture);
            }
        }


        Handles.EndGUI();
    }


    private void WorkSpaceMouseDown(MouseDownEvent evt)
    {

        if (evt.button == 2)//鼠标中键
        {
            mouseCenterDrag = true;
            //Debug.Log(mouseCenterDrag);

        }
        else if(evt.button ==0)
        {
            if (curSelectItem == null) return;

            if (mapData == null) return;
            int x = (int)((evt.localMousePosition.x) / mapEditorConfig.curGridUnitLength);
            int y = (int)((evt.localMousePosition.y) / mapEditorConfig.curGridUnitLength);
            int mapX = (int)(startOffsetX / mapEditorConfig.curGridUnitLength) + x;
            int mapY = (int)(startOffsetY / mapEditorConfig.curGridUnitLength) + y;
            //对于不规整地图视角下的偏移做补正
            if (evt.localMousePosition.x > (x* mapEditorConfig.curGridUnitLength)+(mapEditorConfig.curGridUnitLength - startOffsetX % mapEditorConfig.curGridUnitLength)
                && evt.localMousePosition.x < ((x+1) * mapEditorConfig.curGridUnitLength))
            {
                mapX++;
            }
            if (evt.localMousePosition.y > (y * mapEditorConfig.curGridUnitLength) + (mapEditorConfig.curGridUnitLength - startOffsetY % mapEditorConfig.curGridUnitLength)
                && evt.localMousePosition.y < ((y + 1) * mapEditorConfig.curGridUnitLength))
            {
                mapY++;
            }

            if(itemMenuType == ItemMenuType.Tile)
            {
                TileData tileData = mapEditorItemGroupConfig.TileList.Find(x => x.tileName == curSelectItem.ItemName);
                if (tileData == null) return;
                MapTile mapTile = new MapTile();
                mapTile.tileData = tileData;
                mapTile.mapPos = new GridPos(mapX, mapY);
                if (mapData.tileList.Find(x => x.mapPos.Equals(new GridPos(mapX, mapY))) != null) return;
                mapData.tileList.Add(mapTile);
            }
            else if(itemMenuType == ItemMenuType.Furniture)
            {
                FurnitureData furnitureData = mapEditorItemGroupConfig.FrunitureList.Find(x => x.FurnitureName == curSelectItem.ItemName);
                if (furnitureData == null) return;
                MapFurniture mapFurniture = new MapFurniture();
                mapFurniture.furnitureData = furnitureData;
                mapFurniture.mapPosList = new List<GridPos>();
                foreach (var pos in furnitureData.posList)
                {
                    int GridPosX = pos.x + mapX;
                    int GridPosY = pos.y + mapY;
                    GridPos mapPos = new GridPos(GridPosX, GridPosY);
                    //家具要摆的位置还没有瓦片
                    if (mapData.tileList.Find(x => x.mapPos.Equals(mapPos)) == null) return;
                    mapFurniture.mapPosList.Add(mapPos);
                }
                if (mapData.furnitureList.Find(x => x.mapPosList.Contains(new GridPos(mapX, mapY))) != null) return;
                mapData.furnitureList.Add(mapFurniture);
            }
        }
        else if(evt.button == 1)
        {
            if (mapData == null) return;
            int x = (int)((evt.localMousePosition.x) / mapEditorConfig.curGridUnitLength);
            int y = (int)((evt.localMousePosition.y) / mapEditorConfig.curGridUnitLength);
            int mapX = (int)(startOffsetX / mapEditorConfig.curGridUnitLength) + x;
            int mapY = (int)(startOffsetY / mapEditorConfig.curGridUnitLength) + y;
            //对于不规整地图视角下的偏移做补正
            if (evt.localMousePosition.x > (x * mapEditorConfig.curGridUnitLength) + (mapEditorConfig.curGridUnitLength - startOffsetX % mapEditorConfig.curGridUnitLength)
                && evt.localMousePosition.x < ((x + 1) * mapEditorConfig.curGridUnitLength))
            {
                mapX++;
            }
            if (evt.localMousePosition.y > (y * mapEditorConfig.curGridUnitLength) + (mapEditorConfig.curGridUnitLength - startOffsetY % mapEditorConfig.curGridUnitLength)
                && evt.localMousePosition.y < ((y + 1) * mapEditorConfig.curGridUnitLength))
            {
                mapY++;
            }

            MapFurniture mapFurniture = mapData.furnitureList.Find(x => x.mapPosList.Contains(new GridPos(mapX, mapY)));
            if (mapFurniture != null)
            {
                mapData.furnitureList.Remove(mapFurniture);
            }
            else
            {
                MapTile mapTile = mapData.tileList.Find(x => x.mapPos.Equals(new GridPos(mapX, mapY)));
                if(mapTile!=null)
                {
                    mapData.tileList.Remove(mapTile);
                }
            }
        }

        WorkContainer.MarkDirtyLayout();
    }

    private void WorkSpaceMouseMove(MouseMoveEvent evt)
    {
        if (mouseCenterDrag)
        {
            float height = WorkContainer.contentRect.height > MapEditorConfig.maxMapSizeY ? MapEditorConfig.maxMapSizeY : WorkContainer.contentRect.height;
            float width = WorkContainer.contentRect.width > MapEditorConfig.maxMapSizeX ? MapEditorConfig.maxMapSizeX : WorkContainer.contentRect.width;
            float maxHeight = mapEditorConfig.curGridUnitLength * (MapEditorConfig.maxMapSizeY / MapEditorConfig.minGridUnitLength);
            float maxWidth = mapEditorConfig.curGridUnitLength * (MapEditorConfig.maxMapSizeX / MapEditorConfig.minGridUnitLength);
            //Debug.Log(evt.mouseDelta);
            //Debug.Log(maxWidth - width);
            startOffsetX = Mathf.Clamp(startOffsetX - evt.mouseDelta.x ,0, maxWidth-width);
            startOffsetY = Mathf.Clamp(startOffsetY - evt.mouseDelta.y,0, maxHeight - height);
            //Debug.Log(startOffsetX + "," + startOffsetY);
            UpdateWorkSpaceView();
        }
    }

    private void WorkSpaceMouseUp(MouseUpEvent evt)
    {
        if(evt.button == 2)
        {
            if(mouseCenterDrag)
            {
                mouseCenterDrag = false;
            }
        }
    }

    private void WorkSpaceMouseOut(MouseOutEvent evt)
    {
        if (mouseCenterDrag)
        {
            mouseCenterDrag = false;
        }
    }

    #endregion
}



public enum ItemMenuType
{
    Tile,
    Furniture
}

public class MapEditorConfig
{
    public const int standardGridUnitLength = 40;  // 标准网格单位边长
    public const int maxGridUnitLength = 60;      //  最大网格单位边长
    public const int minGridUnitLength = 20;      //  最小网格单位边长
    public const int maxMapSizeX = 900;
    public const int maxMapSizeY = 600;
    public int curGridUnitLength = 40;             //  当前网格单位边长
}
