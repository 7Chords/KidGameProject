using KidGame.Core;
using KidGame.Core.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;


namespace KidGame.Editor
{
    /// <summary>
    /// 关卡资源配置器编辑器窗口
    /// </summary>
    public class LevelResEditorWindow : EditorWindow
    {
        public static LevelResEditorWindow Instance;

        private LevelResMapEditorConfig mapEditorConfig = new LevelResMapEditorConfig();

        public Color unSelectColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
        public Color selectColor = Color.yellow;

        public static List<Color> colorList = new List<Color>()
        {
            MapEditorColorCfg.color_light_green,
            MapEditorColorCfg.color_light_blue,
            MapEditorColorCfg.color_red,
            MapEditorColorCfg.color_purple,
            MapEditorColorCfg.color_yellow,
            MapEditorColorCfg.color_brown,
            MapEditorColorCfg.color_dark_blue,
            //etc
        };

        [MenuItem("自定义编辑器/关卡配置器")]
        public static void ShowExample()
        {
            LevelResEditorWindow wnd = GetWindow<LevelResEditorWindow>();
            wnd.titleContent = new GUIContent("关卡配置器");
        }

        private VisualElement root;

        public void CreateGUI()
        {
            Instance = this;

            root = rootVisualElement;
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/UnityEditor/LevelResEditor/EditorWindow/LevelResEditorWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            InitTopMenu();
            InitItemMenu();
            InitWorkSpace();
        }

        public void ResetView()
        {

        }


        #region TopMenu
        private Button InfoButton;
        private ObjectField LevelResDataField; //关卡资源数据配置
        private ObjectField MapDataField; //关卡资源数据配置

        private GameLevelData gameLevelData;//当前的关卡资源数据
        private MapData mapData;

        private void InitTopMenu()
        {
            InfoButton = root.Q<Button>(nameof(InfoButton));
            InfoButton.clicked += OnInfoButtonClicked;

            LevelResDataField = root.Q<ObjectField>(nameof(LevelResDataField));
            LevelResDataField.objectType = typeof(GameLevelData);
            LevelResDataField.RegisterValueChangedCallback(LevelResDataFieldValueChanged);
            LevelResDataField.RegisterCallback<MouseDownEvent>(OnLevelResDataFieldClicked);

            MapDataField = root.Q<ObjectField>(nameof(MapDataField));
            MapDataField.objectType = typeof(MapData);
            MapDataField.RegisterValueChangedCallback(MapDataFieldValueChanged);
            MapDataField.RegisterCallback<MouseDownEvent>(OnMapDataFieldClicked);
        }


        private void OnInfoButtonClicked()
        {
            Debug.Log("点击了关卡资源编辑器操作提示按钮！");
        }

        private void LevelResDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            gameLevelData = evt.newValue as GameLevelData;
            UpdateWorkSpaceView();
        }
        private void OnLevelResDataFieldClicked(MouseDownEvent evt)
        {
            if (gameLevelData != null)
            {
                Selection.activeObject = gameLevelData; //设置Inspector显示
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
                Selection.activeObject = mapData; //设置Inspector显示
            }
        }

        /// <summary>
        /// 保存配置数据
        /// </summary>
        public void SaveConfig()
        {
            if (gameLevelData != null)
            {
                EditorUtility.SetDirty(gameLevelData);
                AssetDatabase.SaveAssetIfDirty(gameLevelData);
            }
        }

        #endregion


        #region ItemMenu
        private VisualElement ItemListView;
        private List<LevelEditorItem> editorItemList;
        private Button LeftButton;
        private Button RightButton;
        private Label ItemGroupTitle;

        private LevelEditorItem curSelectItem;
        private ResConfigMenuType itemMenuType;
        private int itemMenuIdx;
        private void InitItemMenu()
        {
            ItemListView = root.Q<VisualElement>(nameof(ItemListView));
            LeftButton = root.Q<Button>(nameof(LeftButton));
            LeftButton.clicked += OnLeftButtonClicked;

            RightButton = root.Q<Button>(nameof(RightButton));
            RightButton.clicked += OnRightButtonClicked;

            ItemGroupTitle = root.Q<Label>(nameof(ItemGroupTitle));

            editorItemList = new List<LevelEditorItem>();
            SetItemMenu(0);

        }

        private void OnRightButtonClicked()
        {
            itemMenuIdx = (itemMenuIdx + 1) > 1 ? 0 : itemMenuIdx + 1;
            itemMenuType = (ResConfigMenuType)itemMenuIdx;
            SetItemMenu(itemMenuType);
        }

        private void OnLeftButtonClicked()
        {
            itemMenuIdx = (itemMenuIdx - 1) < 0 ? 1 : itemMenuIdx - 1;
            itemMenuType = (ResConfigMenuType)itemMenuIdx;
            SetItemMenu(itemMenuType);
        }
        private void SetItemMenu(ResConfigMenuType menuType)
        {
            itemMenuType = menuType;
            switch (itemMenuType)
            {
                case ResConfigMenuType.Logic:
                    ItemGroupTitle.text = "逻辑相关";
                    break;
                case ResConfigMenuType.Material:
                    ItemGroupTitle.text = "材料生成点";
                    break;
                default:
                    break;
            }

            curSelectItem = null;
            RefreshItemGroupView();
        }
        public void SelectOneItem(LevelEditorItem selectItem)
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

        private void RefreshItemGroupView()
        {
            ClearItemGroupView();
            if (itemMenuType == ResConfigMenuType.Logic)
            {
                //for (int i = 0; i < mapEditorItemGroupConfig.TileList.Count; i++)
                //{
                //    MapEditorItem editItem = new MapEditorItem();
                //    editorItemList.Add(editItem);
                //    editItem.Init(ItemListView, mapEditorItemGroupConfig.TileList[i].tileName);
                //}
            }
            else if (itemMenuType == ResConfigMenuType.Material)
            {
                List<MaterialData> dataList = Resources.Load<kidgame_game_data_config>("ScriptObject/kidgame_game_data_config").MaterialDataList;
                for (int i = 0; i < dataList.Count; i++)
                {
                    LevelEditorItem editItem = new LevelEditorItem();
                    editorItemList.Add(editItem);
                    editItem.Init(ItemListView, dataList[i].materialName);
                }
            }
        }

        private void ClearItemGroupView()
        {
            if (editorItemList == null)
            {
                editorItemList = new List<LevelEditorItem>();
                return;
            }
            foreach (var item in editorItemList)
            {
                item.Destory();
            }

            editorItemList.Clear();
        }



        public void SetButtonBorderColor(Button btn, Color color)
        {
            btn.style.borderBottomColor = color;
            btn.style.borderTopColor = color;
            btn.style.borderLeftColor = color;
            btn.style.borderRightColor = color;
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
            mapEditorConfig.curGridUnitLength = Mathf.Clamp(mapEditorConfig.curGridUnitLength - delta
                , LevelResMapEditorConfig.minGridUnitLength, LevelResMapEditorConfig.maxGridUnitLength);
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

            float height = rect.height > LevelResMapEditorConfig.maxMapSizeY ? LevelResMapEditorConfig.maxMapSizeY : rect.height;
            float width = rect.width > LevelResMapEditorConfig.maxMapSizeX ? LevelResMapEditorConfig.maxMapSizeX : rect.width;

            //画横线
            float startY = startOffsetY % mapEditorConfig.curGridUnitLength;
            for (float i = mapEditorConfig.curGridUnitLength - startY;
                 i <= height;
                 i += mapEditorConfig.curGridUnitLength)
            {
                Handles.DrawLine(new Vector3(0, i),
                    new Vector3(width, i));
            }

            //画竖线
            float startX = startOffsetX % mapEditorConfig.curGridUnitLength;
            for (float i = mapEditorConfig.curGridUnitLength - startX;
                 i <= width;
                 i += mapEditorConfig.curGridUnitLength)
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
                    GUI.color = colorList[(int)tile.roomType];
                    GUI.DrawTexture(new Rect((tile.mapPos.x - offsetGridX) * mapEditorConfig.curGridUnitLength,
                        (tile.mapPos.y - offsetGridY) * mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength), tile.tileData.texture);
                }

                GUI.color = Color.white;
                foreach (var furniture in mapData.furnitureList)
                {
                    int mapXMin = 999, mapXMax = -1, mapYMin = 999, mapYMax = -1;
                    foreach (var pos in furniture.mapPosList)
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
                        (mapXMax - mapXMin + 1) * mapEditorConfig.curGridUnitLength,
                        (mapYMax - mapYMin + 1) * mapEditorConfig.curGridUnitLength), furniture.furnitureData.texture);
                }

                foreach (var wall in mapData.wallList)
                {
                    int mapXMin = 999, mapXMax = -1, mapYMin = 999, mapYMax = -1;
                    foreach (var pos in wall.mapPosList)
                    {
                        if (pos.x < mapXMin) mapXMin = pos.x;
                        if (pos.x > mapXMax) mapXMax = pos.x;
                        if (pos.y < mapYMin) mapYMin = pos.y;
                        if (pos.y > mapYMax) mapYMax = pos.y;
                    }

                    float offsetGridX = startOffsetX / mapEditorConfig.curGridUnitLength;
                    float offsetGridY = startOffsetY / mapEditorConfig.curGridUnitLength;
                    //画墙的图
                    if ((mapXMin - offsetGridX) < 0 || (mapYMin - offsetGridY) < 0) continue;
                    GUI.DrawTexture(new Rect((mapXMin - offsetGridX) * mapEditorConfig.curGridUnitLength,
                        (mapYMin - offsetGridY) * mapEditorConfig.curGridUnitLength,
                        (mapXMax - mapXMin + 1) * mapEditorConfig.curGridUnitLength,
                        (mapYMax - mapYMin + 1) * mapEditorConfig.curGridUnitLength), wall.wallData.texture);
                    //画墙的数量
                    GUIStyle labelStyle = new GUIStyle();
                    labelStyle.fontSize = 30 * (mapEditorConfig.curGridUnitLength / MapEditorConfig.maxGridUnitLength);
                    GUI.Label(new Rect((mapXMin - offsetGridX) * mapEditorConfig.curGridUnitLength,
                        (mapYMin - offsetGridY) * mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength), wall.stackLayer.ToString(), labelStyle);
                }
            }


            Handles.EndGUI();
        }

        private void WorkSpaceMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 2) //鼠标中键
            {
                mouseCenterDrag = true;
                //Debug.Log(mouseCenterDrag);
            }

            WorkContainer.MarkDirtyLayout();
        }

        private void WorkSpaceMouseMove(MouseMoveEvent evt)
        {
            if (mouseCenterDrag)
            {
                float height = WorkContainer.contentRect.height > LevelResMapEditorConfig.maxMapSizeY
                    ? LevelResMapEditorConfig.maxMapSizeY
                    : WorkContainer.contentRect.height;
                float width = WorkContainer.contentRect.width > LevelResMapEditorConfig.maxMapSizeX
                    ? LevelResMapEditorConfig.maxMapSizeX
                    : WorkContainer.contentRect.width;
                float maxHeight = mapEditorConfig.curGridUnitLength *
                                  (LevelResMapEditorConfig.maxMapSizeY / LevelResMapEditorConfig.minGridUnitLength);
                float maxWidth = mapEditorConfig.curGridUnitLength *
                                 (LevelResMapEditorConfig.maxMapSizeX / LevelResMapEditorConfig.minGridUnitLength);
                //Debug.Log(evt.mouseDelta);
                //Debug.Log(maxWidth - width);
                startOffsetX = Mathf.Clamp(startOffsetX - evt.mouseDelta.x, 0, maxWidth - width);
                startOffsetY = Mathf.Clamp(startOffsetY - evt.mouseDelta.y, 0, maxHeight - height);
                //Debug.Log(startOffsetX + "," + startOffsetY);
                UpdateWorkSpaceView();
            }
        }

        private void WorkSpaceMouseUp(MouseUpEvent evt)
        {
            if (evt.button == 2)
            {
                if (mouseCenterDrag)
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

    public enum ResConfigMenuType
    {
        Material = 0,
        Logic = 1,//逻辑点配置，如敌人生成位置
    }

    public class LevelResMapEditorConfig
    {
        public const int standardGridUnitLength = 40; // 标准网格单位边长
        public const int maxGridUnitLength = 60; //  最大网格单位边长
        public const int minGridUnitLength = 20; //  最小网格单位边长
        public const int maxMapSizeX = 900;
        public const int maxMapSizeY = 600;
        public int curGridUnitLength = 40; //  当前网格单位边长
    }
}
