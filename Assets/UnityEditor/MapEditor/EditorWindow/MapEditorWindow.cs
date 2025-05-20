using System;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;
using UnityEditor.Searcher;
/**
 * 地图编辑器窗口脚本
 */
public class MapEditorWindow : EditorWindow
{
    public static MapEditorWindow Instance;

    private MapEditorConfig mapEditorConfig = new MapEditorConfig();

    [MenuItem("自定义编辑器/地图编辑器")]
    public static void ShowExample()
    {
        MapEditorWindow wnd = GetWindow<MapEditorWindow>();
        wnd.titleContent = new GUIContent("地图编辑器");
    }

    private VisualElement root;
    public void CreateGUI()
    {
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
        EditRoomButton.style.color = Color.yellow;
        EditHouseButton.style.color = Color.white;

    }

    private void OnEditHouseButtonClicked()
    {
        editorMode = MapEditorMode.House;
        EditHouseButton.style.color = Color.yellow;
        EditRoomButton.style.color = Color.white;
    }

    private void OnInfoButtonClicked()
    {
        Debug.Log("InfoButtonClick");
    }


    private void RoomDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        roomData = evt.newValue as RoomData;
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

    private void InitItemMenu()
    {
        ItemListView = root.Q<VisualElement>(nameof(ItemListView));
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
        //WorkContainer.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
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
    }
    private void UpdateWorkSpaceView()
    {
        WorkContainer.MarkDirtyLayout(); // 标志为需要重新绘制的
    }
    private void DrawWorkSpace()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;

        Rect rect = WorkContainer.contentRect;
        int row = (int)rect.height / mapEditorConfig.curGridUnitLength;//行
        int column = (int)rect.width / mapEditorConfig.curGridUnitLength;//列

        int curStartPos = 0;
        for(int i=0;i<row;i++)
        {
            Handles.DrawLine(new Vector3(0, curStartPos + i * mapEditorConfig.curGridUnitLength),
                new Vector3(rect.width, curStartPos + i * mapEditorConfig.curGridUnitLength));
            curStartPos += mapEditorConfig.curGridUnitLength;
        }
        curStartPos = 0;
        for (int i = 0; i < column; i++)
        {
            Handles.DrawLine(new Vector3(curStartPos + i * mapEditorConfig.curGridUnitLength, 0),
                new Vector3(curStartPos + i * mapEditorConfig.curGridUnitLength, rect.width));
            curStartPos += mapEditorConfig.curGridUnitLength;
        }
         
        //Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
        //string indexStr = index.ToString();
        //GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);




        //// 起始索引
        //int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
        //// 计算绘制起点的偏移
        //float startOffset = 0;
        //if (index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);

        ////相隔多少刻度画一个长线
        //int tickStep = SkillEditorConfig.maxFrameWidthLV + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.standFrameUnitWidth);
        //tickStep = tickStep / 2; // 可能 1 / 2 = 0的情况
        //if (tickStep == 0) tickStep = 1; // 避免为0
        //for (float i = startOffset; i < rect.width; i += skillEditorConfig.frameUnitWidth)
        //{
        //    // 绘制长线、文本
        //    if (index % tickStep == 0)
        //    {
        //        Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
        //        string indexStr = index.ToString();
        //        GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
        //    }
        //    else Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
        //    index += 1;
        //}
        Handles.EndGUI();
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
    public const int standardGridUnitLength = 20;  // 标准网格单位边长
    public const int maxGridUnitLength = 40;      //  最大网格单位边长
    public const int minGridUnitLength = 10;      //  最小网格单位边长
    public int curGridUnitLength = 20;             //  当前网格单位边长
}
