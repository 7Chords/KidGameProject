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
    private Button EditRoomButton;
    private Button EditHouseButton;
    private Button InfoButton;


    //ObjectField是
    private ObjectField RoomDataField;
    private ObjectField MapDataField;
    private ObjectField ItemConfigField;//菜单栏配置数据

    private MapEditorMode editorMode;

    private RoomData roomData;
    private MapData mapData;
    private MapEditorItemGroupConfig mapEditorItemGroupConfig;

    private void InitTopMenu()
    {
        EditRoomButton = root.Q<Button>(nameof(EditRoomButton));
        EditRoomButton.clicked += OnEditRoomButtonClicked;

        EditHouseButton = root.Q<Button>(nameof(EditHouseButton));
        EditHouseButton.clicked += OnEditHouseButtonClicked;

        InfoButton = root.Q<Button>(nameof(InfoButton));
        InfoButton.clicked += OnInfoButtonClicked;

        //ObjectField是UnityEditor.UIElements.ObjectField
        RoomDataField = root.Q<ObjectField>(nameof(RoomDataField));
        RoomDataField.objectType = typeof(RoomData);
        RoomDataField.RegisterValueChangedCallback(RoomDataFieldValueChanged);
        RoomDataField.RegisterCallback<MouseDownEvent>(OnRoomDataFieldClicked);

        MapDataField = root.Q<ObjectField>(nameof(MapDataField));
        MapDataField.objectType = typeof(MapData);
        MapDataField.RegisterValueChangedCallback(MapDataFieldValueChanged);
        MapDataField.RegisterCallback<MouseDownEvent>(OnMapDataFieldClicked);

        ItemConfigField = root.Q<ObjectField>(nameof(ItemConfigField));
        ItemConfigField.objectType = typeof(MapEditorItemGroupConfig);
        ItemConfigField.RegisterValueChangedCallback(ItemConfigFieldValueChanged);
        ItemConfigField.RegisterCallback<MouseDownEvent>(OnItemConfigFieldClicked);

        //默认是房间编辑模式
        OnEditRoomButtonClicked();
    }

    private void OnEditRoomButtonClicked()
    {
        editorMode = MapEditorMode.Room;
        SetButtonBorderColor(EditRoomButton, selectColor);
        SetButtonBorderColor(EditHouseButton, unSelectColor);
        UpdateWorkSpaceView();
    }
    private void OnEditHouseButtonClicked()
    {
        editorMode = MapEditorMode.House;
        SetButtonBorderColor(EditRoomButton, unSelectColor);
        SetButtonBorderColor(EditHouseButton, selectColor);
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
        Debug.Log("InfoButtonClick");
    }


    private void RoomDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        roomData = evt.newValue as RoomData;
        UpdateWorkSpaceView();
    }
    private void OnRoomDataFieldClicked(MouseDownEvent evt)
    {
        if(roomData != null)
        {
            Selection.activeObject = roomData;
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
            Selection.activeObject = mapData;
        }
    }
    private void ItemConfigFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        mapEditorItemGroupConfig = evt.newValue as MapEditorItemGroupConfig;
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
        
    }

    private void OnRightButtonClicked()
    {
        
    }

    private void RefreshItemGroupView()
    {
        if (mapEditorItemGroupConfig == null)
        {
            ClearItemGroupView();
            return;
        }
        if(editorMode == MapEditorMode.Room)
        {
            for (int i = 0; i < mapEditorItemGroupConfig.TileList.Count; i++)
            {
                MapEditorItem editItem = new MapEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView, mapEditorItemGroupConfig.TileList[i].tileName);
            }
            //for (int i = 0; i < mapEditorItemGroupConfig.FrunitureList.Count;i++)
            //{
            //    MapEditorItem editItem = new MapEditorItem();
            //    editorItemList.Add(editItem);
            //    editItem.Init(ItemListView, mapEditorItemGroupConfig.FrunitureList[i].FurnitureName);
            //}
        }
        else if(editorMode == MapEditorMode.House)
        {
            for (int i = 0; i < mapEditorItemGroupConfig.RoomList.Count; i++)
            {
                MapEditorItem editItem = new MapEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView, mapEditorItemGroupConfig.RoomList[i].RoomName);
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


    private void InitWorkSpace()
    {
        WorkContainer = root.Q<IMGUIContainer>(nameof(WorkContainer));
        //IMGUI的绘制函数
        WorkContainer.onGUIHandler = DrawWorkSpace;
        WorkContainer.RegisterCallback<WheelEvent>(WorkSpaceWheel);
        WorkContainer.RegisterCallback<MouseDownEvent>(WorkSpaceMouseDown);
        //WorkContainer.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
        //WorkContainer.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
        //WorkContainer.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);
    }
    private void RefreshWorkSpace()
    {

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
        int row = (int)rect.height / mapEditorConfig.curGridUnitLength;//行
        int column = (int)rect.width / mapEditorConfig.curGridUnitLength;//列

        int curStartPos = 0;
        for(int i=0;i<=row;i++)
        {
            Handles.DrawLine(new Vector3(0, curStartPos /*+ i * mapEditorConfig.curGridUnitLength*/),
                new Vector3(rect.width, curStartPos /*+ i * mapEditorConfig.curGridUnitLength*/));
            curStartPos += mapEditorConfig.curGridUnitLength;
        }
        curStartPos = 0;
        for (int i = 0; i <= column; i++)
        {
            Handles.DrawLine(new Vector3(curStartPos /*+ i * mapEditorConfig.curGridUnitLength*/, 0),
                new Vector3(curStartPos /*+ i * mapEditorConfig.curGridUnitLength*/, rect.width));
            curStartPos += mapEditorConfig.curGridUnitLength;
        }

        //画数据

        if(editorMode == MapEditorMode.Room)
        {
            if(roomData!=null)
            {
                foreach(var item in roomData.itemList)
                {
                    foreach (var pos in item.mapPosList)
                    {
                        GUIStyle labelStyle = new GUIStyle();
                        labelStyle.fontSize = 20;
                        GUI.Label(new Rect(pos.x * mapEditorConfig.curGridUnitLength,
                            pos.y * mapEditorConfig.curGridUnitLength,
                            mapEditorConfig.curGridUnitLength,
                            mapEditorConfig.curGridUnitLength/2), item.tileData.tileName);
                    }

                }
            }
        }


        Handles.EndGUI();
    }


    private void WorkSpaceMouseDown(MouseDownEvent evt)
    {

        if (curSelectItem == null) return;
        if ((editorMode == MapEditorMode.Room && roomData == null) || (editorMode == MapEditorMode.House && mapData == null)) return;
        int x = (int)(evt.localMousePosition.x / mapEditorConfig.curGridUnitLength);
        int y = (int)(evt.localMousePosition.y / mapEditorConfig.curGridUnitLength);

        if(editorMode==MapEditorMode.Room)
        {
            //FurnitureData furnitureData = mapEditorItemGroupConfig.FrunitureList.Find(x => x.FurnitureName == curSelectItem.ItemName);
            //if (furnitureData == null) return;
            TileData tileData = mapEditorItemGroupConfig.TileList.Find(x => x.tileName == curSelectItem.ItemName);
            if (tileData == null) return;
            MapItem mapItem = new MapItem();
            mapItem.gridState = MapGridState.Tile;
            mapItem.tileData = tileData;
            mapItem.mapPosList = new List<GridPos>();
            foreach (var pos in tileData.posList)
            {
                int mapX = pos.x + x;
                int mapY = pos.y + y;
                // 记录已绘制的网格状态
                GridPos mapPos = new GridPos(mapX, mapY);
                mapItem.mapPosList.Add(mapPos);
            }
            roomData.itemList.Add(mapItem);

        }
        else if(editorMode == MapEditorMode.House)
        {

        }
        WorkContainer.MarkDirtyLayout();

    }

    #endregion
}
public enum MapEditorMode
{
    Room,
    House,
}




public class MapEditorConfig
{
    public const int standardGridUnitLength = 40;  // 标准网格单位边长
    public const int maxGridUnitLength = 60;      //  最大网格单位边长
    public const int minGridUnitLength = 20;      //  最小网格单位边长
    public int curGridUnitLength = 40;             //  当前网格单位边长
}
