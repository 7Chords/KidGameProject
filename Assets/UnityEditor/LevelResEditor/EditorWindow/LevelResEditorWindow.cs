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
    /// �ؿ���Դ�������༭������
    /// </summary>
    public class LevelResEditorWindow : EditorWindow
    {
        public static LevelResEditorWindow Instance;

        private LevelResMapEditorConfig mapEditorConfig = new LevelResMapEditorConfig();

        public Color unSelectColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
        public Color selectColor = Color.yellow;

        public const float ENEMY_SPAWN_Y_OFFSET = 2;
        public const float MATERIAL_SPAWN_Y_OFFSET = 1;
        public const float Player_SPAWN_Y_OFFSET = 2;


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

        private List<MaterialData> dataList;
        //ctrl + shift + o
        [MenuItem("�Զ���༭��/�ؿ������� %#o")]
        public static void ShowExample()
        {
            LevelResEditorWindow wnd = GetWindow<LevelResEditorWindow>();
            wnd.titleContent = new GUIContent("�ؿ�������");
        }

        private VisualElement root;

        private LevelConfigModeType currentMode;

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
            InitF2MConfigSpace();

            ResetView();

            SetConfigMode(LevelConfigModeType.F2M);
        }

        public void ResetView()
        {
            MapData tmpMapData = mapData;
            MapDataField.value = null;
            MapDataField.value = tmpMapData;

            GameLevelData tmpLevelData = gameLevelData;
            LevelResDataField.value = null;
            LevelResDataField.value = tmpLevelData;

            curMapFurnitureData = null;
            curSelectItem = null;
        }

        #region TopMenu
        private Button InfoButton;
        private ObjectField LevelResDataField; //�ؿ���Դ��������
        private ObjectField MapDataField; //�ؿ���Դ��������

        private GameLevelData gameLevelData;//��ǰ�Ĺؿ���Դ����
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
            Debug.Log("����˹ؿ���Դ�༭��������ʾ��ť��");
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
                Selection.activeObject = gameLevelData; //����Inspector��ʾ
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
                Selection.activeObject = mapData; //����Inspector��ʾ
            }
        }

        /// <summary>
        /// ������������
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
                    ItemGroupTitle.text = "�߼����";
                    break;
                case ResConfigMenuType.Material:
                    ItemGroupTitle.text = "�������ɵ�";
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

            SetConfigMode(selectItem.ConfigType);

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
                LevelEditorItem editItem = new LevelEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView,LevelConfigModeType.Enemy, "EnemySpawnPoint", "�������ɵ�", -1);

                editItem = new LevelEditorItem();
                editorItemList.Add(editItem);
                editItem.Init(ItemListView, LevelConfigModeType.Player,"PlayerSpawnPoint", "������ɵ�", -1);

            }
            else if (itemMenuType == ResConfigMenuType.Material)
            {
                dataList = Resources.Load<kidgame_game_data_config>("ScriptObject/kidgame_game_data_config").MaterialDataList;
                for (int i = 0; i < dataList.Count; i++)
                {
                    LevelEditorItem editItem = new LevelEditorItem();
                    editorItemList.Add(editItem);
                    editItem.Init(ItemListView, LevelConfigModeType.R2M, dataList[i].id,dataList[i].materialName, dataList[i].rare);
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

        private int serialNumber;

        private MapFurnitureData curMapFurnitureData;
        private void InitWorkSpace()
        {
            WorkContainer = root.Q<IMGUIContainer>(nameof(WorkContainer));
            //IMGUI�Ļ��ƺ���
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
        }


        private void UpdateWorkSpaceView()
        {
            if (WorkContainer == null) return;
            WorkContainer.MarkDirtyLayout(); // ��־Ϊ��Ҫ���»��Ƶ�
        }

        private void DrawWorkSpace()
        {
            Handles.BeginGUI();
            Handles.color = Color.white;

            // ��ȡ����������
            Rect rect = WorkContainer.contentRect;

            // ����ʵ�ʻ��Ʒ�Χ������������ͼ�ߴ磩
            float visibleHeight = Mathf.Min(rect.height, LevelResMapEditorConfig.maxMapSizeY);
            float visibleWidth = Mathf.Min(rect.width, LevelResMapEditorConfig.maxMapSizeX);

            // ���㵱ǰ�ɼ�������������귶Χ�����ǹ���ƫ�ƣ�
            float startGridX = startOffsetX / mapEditorConfig.curGridUnitLength;
            float startGridY = startOffsetY / mapEditorConfig.curGridUnitLength;
            float endGridX = startGridX + (visibleWidth / mapEditorConfig.curGridUnitLength);
            float endGridY = startGridY + (visibleHeight / mapEditorConfig.curGridUnitLength);

            // ���������
            float startY = startOffsetY % mapEditorConfig.curGridUnitLength;
            for (float y = mapEditorConfig.curGridUnitLength - startY; y <= visibleHeight; y += mapEditorConfig.curGridUnitLength)
            {
                Handles.DrawLine(new Vector3(0, y), new Vector3(visibleWidth, y));
            }

            // ����������
            float startX = startOffsetX % mapEditorConfig.curGridUnitLength;
            for (float x = mapEditorConfig.curGridUnitLength - startX; x <= visibleWidth; x += mapEditorConfig.curGridUnitLength)
            {
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, visibleHeight));
            }

            // ������
            if (mapData != null)
            {
                // ������Ƭ
                foreach (var tile in mapData.tileList)
                {
                    // �����Ƭ�Ƿ��ڿɼ���Χ��
                    if (tile.mapPos.x < startGridX || tile.mapPos.x > endGridX ||
                        tile.mapPos.y < startGridY || tile.mapPos.y > endGridY)
                    {
                        continue;
                    }

                    // ������Ļ����
                    float screenX = (tile.mapPos.x - startGridX) * mapEditorConfig.curGridUnitLength;
                    float screenY = (tile.mapPos.y - startGridY) * mapEditorConfig.curGridUnitLength;

                    // ��Ƭ����һ��
                    GUI.color = new Color(colorList[(int)tile.roomType].r,
                        colorList[(int)tile.roomType].g,
                        colorList[(int)tile.roomType].b, 0.5f);
                    GUI.DrawTexture(new Rect(screenX, screenY,
                        mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength),
                        tile.tileData.texture, ScaleMode.ScaleToFit);
                }

                GUI.color = Color.white;

                // ���ƼҾ�
                foreach (var furniture in mapData.furnitureList)
                {
                    int mapXMin = 999, mapXMax = -1, mapYMin = 999, mapYMax = -1;
                    bool isVisible = false;

                    // ����Ҿߵı߽粢���ɼ���
                    foreach (var pos in furniture.mapPosList)
                    {
                        if (pos.x < mapXMin) mapXMin = pos.x;
                        if (pos.x > mapXMax) mapXMax = pos.x;
                        if (pos.y < mapYMin) mapYMin = pos.y;
                        if (pos.y > mapYMax) mapYMax = pos.y;

                        // ����Ƿ�������һ�����ڿɼ���Χ��
                        if (pos.x >= startGridX && pos.x <= endGridX &&
                            pos.y >= startGridY && pos.y <= endGridY)
                        {
                            isVisible = true;
                        }
                    }

                    // �����ȫ���ɼ�������
                    if (!isVisible) continue;

                    // ������Ļ����
                    float screenX = (mapXMin - startGridX) * mapEditorConfig.curGridUnitLength;
                    float screenY = (mapYMin - startGridY) * mapEditorConfig.curGridUnitLength;
                    float width = (mapXMax - mapXMin + 1) * mapEditorConfig.curGridUnitLength;
                    float height = (mapYMax - mapYMin + 1) * mapEditorConfig.curGridUnitLength;

                    // ȷ��������Ƶ��ɼ�����֮��
                    if (screenX + width < 0 || screenX > visibleWidth ||
                        screenY + height < 0 || screenY > visibleHeight)
                    {
                        continue;
                    }

                    GUI.DrawTexture(new Rect(screenX, screenY, width, height),
                        furniture.furnitureData.texture);
                }

                // ����ǽ
                foreach (var wall in mapData.wallList)
                {
                    int mapXMin = 999, mapXMax = -1, mapYMin = 999, mapYMax = -1;
                    bool isVisible = false;

                    // ����ǽ�ı߽粢���ɼ���
                    foreach (var pos in wall.mapPosList)
                    {
                        if (pos.x < mapXMin) mapXMin = pos.x;
                        if (pos.x > mapXMax) mapXMax = pos.x;
                        if (pos.y < mapYMin) mapYMin = pos.y;
                        if (pos.y > mapYMax) mapYMax = pos.y;

                        // ����Ƿ�������һ�����ڿɼ���Χ��
                        if (pos.x >= startGridX && pos.x <= endGridX &&
                            pos.y >= startGridY && pos.y <= endGridY)
                        {
                            isVisible = true;
                        }
                    }

                    // �����ȫ���ɼ�������
                    if (!isVisible) continue;

                    // ������Ļ����
                    float screenX = (mapXMin - startGridX) * mapEditorConfig.curGridUnitLength;
                    float screenY = (mapYMin - startGridY) * mapEditorConfig.curGridUnitLength;
                    float width = (mapXMax - mapXMin + 1) * mapEditorConfig.curGridUnitLength;
                    float height = (mapYMax - mapYMin + 1) * mapEditorConfig.curGridUnitLength;

                    // ȷ��������Ƶ��ɼ�����֮��
                    if (screenX + width < 0 || screenX > visibleWidth ||
                        screenY + height < 0 || screenY > visibleHeight)
                    {
                        continue;
                    }

                    // ǽ����һ��
                    GUI.color = new Color(1, 1, 1, 0.5f);
                    GUI.DrawTexture(new Rect(screenX, screenY, width, height),
                        wall.wallData.texture);

                    // ��ǽ������
                    if (mapEditorConfig.curGridUnitLength >= MapEditorConfig.maxGridUnitLength * 0.5f)
                    {
                        GUIStyle labelStyle = new GUIStyle();
                        labelStyle.fontSize = (int)(30 * (mapEditorConfig.curGridUnitLength / MapEditorConfig.maxGridUnitLength));
                        GUI.Label(new Rect(screenX, screenY,
                            mapEditorConfig.curGridUnitLength,
                            mapEditorConfig.curGridUnitLength),
                            wall.stackLayer.ToString(), labelStyle);
                    }
                }
            }

            // ���Ƶ�ͼ��ɢ��Ĳ��Ϻ͵�����������õ�
            if(gameLevelData != null)
            {
                GUI.color = Color.white;
                Texture2D tex = Resources.Load<Texture2D>("GUI/Editor/MaterialConfigTex");
                GUIStyle labelStyle = new GUIStyle();
                labelStyle.fontSize = (int)(30 * (mapEditorConfig.curGridUnitLength / MapEditorConfig.maxGridUnitLength));
                MaterialData tmpData;
                dataList = Resources.Load<kidgame_game_data_config>("ScriptObject/kidgame_game_data_config").MaterialDataList;
                float screenX, screenY;
                foreach (var mapping in gameLevelData.r2MMappingList)
                {
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    tmpData = dataList.Find(x => x.id == mapping.materialId);
                    // ������Ļ����
                    screenX = (mapping.spawnPos.x - startGridX) * mapEditorConfig.curGridUnitLength;
                    screenY = (mapping.spawnPos.z - startGridY) * mapEditorConfig.curGridUnitLength;
                    GUI.DrawTexture(new Rect(screenX, screenY,
                         mapEditorConfig.curGridUnitLength,
                         mapEditorConfig.curGridUnitLength),
                         tex, ScaleMode.ScaleToFit);
                    GUI.Label(new Rect(screenX, screenY,
                        mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength),
                        tmpData.materialName, labelStyle);
                    labelStyle.alignment = TextAnchor.LowerRight;
                    GUI.Label(new Rect(screenX, screenY,
                        mapEditorConfig.curGridUnitLength,
                        mapEditorConfig.curGridUnitLength),
                        "["+ mapping .randomAmount_min+","+ mapping.randomAmount_max + "]", labelStyle);

                }
                GUI.color = Color.red;
                tex = Resources.Load<Texture2D>("GUI/Editor/EnemySpawnConfigTex");
                // ���������ɵ�
                foreach (var cfg in gameLevelData.enemySpawnCfgList)
                {
                    screenX = (cfg.enemySpawnPos.x - startGridX) * mapEditorConfig.curGridUnitLength;
                    screenY = (cfg.enemySpawnPos.z - startGridY) * mapEditorConfig.curGridUnitLength;
                    GUI.DrawTexture(new Rect(screenX, screenY,
                         mapEditorConfig.curGridUnitLength,
                         mapEditorConfig.curGridUnitLength),
                         tex, ScaleMode.ScaleToFit);
                }
                GUI.color = Color.green;
                tex = Resources.Load<Texture2D>("GUI/Editor/PlayerSpawnConfigTex");
                screenX = (gameLevelData.playerSpawnPos.x - startGridX) * mapEditorConfig.curGridUnitLength;
                screenY = (gameLevelData.playerSpawnPos.z - startGridY) * mapEditorConfig.curGridUnitLength;
                GUI.DrawTexture(new Rect(screenX, screenY,
                     mapEditorConfig.curGridUnitLength,
                     mapEditorConfig.curGridUnitLength),
                     tex, ScaleMode.ScaleToFit);

            }


            Handles.EndGUI();
        }

        private void WorkSpaceMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 2) //����м�
            {
                mouseCenterDrag = true;
            }
            else if(evt.button == 0)//������
            {
                if (mapData == null) return;
                if (gameLevelData == null) return;

                int x = (int)((evt.localMousePosition.x) / mapEditorConfig.curGridUnitLength);
                int y = (int)((evt.localMousePosition.y) / mapEditorConfig.curGridUnitLength);
                int mapX = (int)(startOffsetX / mapEditorConfig.curGridUnitLength) + x;
                int mapY = (int)(startOffsetY / mapEditorConfig.curGridUnitLength) + y;
                //���ڲ�������ͼ�ӽ��µ�ƫ��������
                if (evt.localMousePosition.x > (x * mapEditorConfig.curGridUnitLength) +
                    (mapEditorConfig.curGridUnitLength - startOffsetX % mapEditorConfig.curGridUnitLength)
                    && evt.localMousePosition.x < ((x + 1) * mapEditorConfig.curGridUnitLength))
                {
                    mapX++;
                }

                if (evt.localMousePosition.y > (y * mapEditorConfig.curGridUnitLength) +
                    (mapEditorConfig.curGridUnitLength - startOffsetY % mapEditorConfig.curGridUnitLength)
                    && evt.localMousePosition.y < ((y + 1) * mapEditorConfig.curGridUnitLength))
                {
                    mapY++;
                }

                bool clickFurniture = false;
                bool needRefresh = false;
                MapFurnitureData tmpData;
                GridPos mapPos = new GridPos(mapX, mapY);
                for (int i = 0; i < mapData.furnitureList.Count; i++)
                {
                    if (mapData.furnitureList[i].mapPosList.Contains(mapPos))
                    {
                        SetConfigMode(LevelConfigModeType.F2M);
                        clickFurniture = true;
                        tmpData = mapData.furnitureList[i];
                        if (curMapFurnitureData != tmpData)
                        {
                            needRefresh = true;
                            curMapFurnitureData = tmpData;
                        }
                        serialNumber = i;//���кž��ǼҾ��ڵ�ͼ�����б��е�����
                        break;
                    }
                }
                if (needRefresh) UpdateF2MConfigView();
                if (!clickFurniture && curSelectItem != null)
                {
                    switch(curSelectItem.ConfigType)
                    {
                        case LevelConfigModeType.R2M:
                            //�ӵ������б���
                            Room2MaterialMapping mapping;
                            mapping = gameLevelData.r2MMappingList.Find(x => x.spawnPos == new Vector3(mapPos.x, MATERIAL_SPAWN_Y_OFFSET, mapPos.y));
                            if (mapping == null)
                            {
                                //todo:y�������
                                mapping = new Room2MaterialMapping(new Vector3(mapPos.x, MATERIAL_SPAWN_Y_OFFSET, mapPos.y),
                                    curSelectItem.Id, r2MSpawnAmountMin, r2MSpawnAmountMax);
                                gameLevelData.r2MMappingList.Add(mapping);
                            }
                            else
                            {
                                mapping.materialId = curSelectItem.Id;
                                mapping.randomAmount_max = r2MSpawnAmountMax;
                                mapping.randomAmount_min = r2MSpawnAmountMin;
                            }
                            break;
                        case LevelConfigModeType.Enemy:
                            if(enemyData != null)
                            {
                                EnemySpawnCfg enemyCfg;
                                enemyCfg = gameLevelData.enemySpawnCfgList.Find(x => x.enemySpawnPos == new Vector3(mapPos.x, ENEMY_SPAWN_Y_OFFSET, mapPos.y));
                                if (enemyCfg == null)
                                {
                                    enemyCfg = new EnemySpawnCfg();
                                    enemyCfg.enemyData = enemyData;
                                    //todo:y������
                                    enemyCfg.enemySpawnPos = new Vector3(mapPos.x, ENEMY_SPAWN_Y_OFFSET, mapPos.y);
                                    gameLevelData.enemySpawnCfgList.Add(enemyCfg);
                                }
                                else
                                {
                                    enemyCfg.enemyData = enemyData;
                                    enemyCfg.enemySpawnPos = new Vector3(mapPos.x, ENEMY_SPAWN_Y_OFFSET, mapPos.y);
                                }
                            }
                            break;
                        case LevelConfigModeType.Player:
                            gameLevelData.playerSpawnPos = new Vector3(mapPos.x, Player_SPAWN_Y_OFFSET, mapPos.y);
                            break;
                        default:
                            break;
                    }
                }
                SaveConfig();

            }
            else if(evt.button == 1)
            {
                if (mapData == null) return;
                if (gameLevelData == null) return;

                int x = (int)((evt.localMousePosition.x) / mapEditorConfig.curGridUnitLength);
                int y = (int)((evt.localMousePosition.y) / mapEditorConfig.curGridUnitLength);
                int mapX = (int)(startOffsetX / mapEditorConfig.curGridUnitLength) + x;
                int mapY = (int)(startOffsetY / mapEditorConfig.curGridUnitLength) + y;
                //���ڲ�������ͼ�ӽ��µ�ƫ��������
                if (evt.localMousePosition.x > (x * mapEditorConfig.curGridUnitLength) +
                    (mapEditorConfig.curGridUnitLength - startOffsetX % mapEditorConfig.curGridUnitLength)
                    && evt.localMousePosition.x < ((x + 1) * mapEditorConfig.curGridUnitLength))
                {
                    mapX++;
                }

                if (evt.localMousePosition.y > (y * mapEditorConfig.curGridUnitLength) +
                    (mapEditorConfig.curGridUnitLength - startOffsetY % mapEditorConfig.curGridUnitLength)
                    && evt.localMousePosition.y < ((y + 1) * mapEditorConfig.curGridUnitLength))
                {
                    mapY++;
                }

                //�Ƴ�˳�򣺵�������-->������������
                EnemySpawnCfg enemyCfg = gameLevelData.enemySpawnCfgList.Find(x => x.enemySpawnPos == new Vector3(mapX, ENEMY_SPAWN_Y_OFFSET, mapY));
                if(enemyCfg != null)
                {
                    gameLevelData.enemySpawnCfgList.Remove(enemyCfg);
                }
                else
                {
                    Room2MaterialMapping mapping = gameLevelData.r2MMappingList.Find(x => x.spawnPos == new Vector3(mapX, MATERIAL_SPAWN_Y_OFFSET, mapY));
                    if (mapping != null)
                    {
                        gameLevelData.r2MMappingList.Remove(mapping);
                    }
                }
                SaveConfig();
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
                startOffsetX = Mathf.Clamp(startOffsetX - evt.mouseDelta.x, 0, maxWidth - width);
                startOffsetY = Mathf.Clamp(startOffsetY - evt.mouseDelta.y, 0, maxHeight - height);
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


        #region Config Space

        private Label SelectFurnitureLabel;
        private Label FurnitureGridLabel;

        private Button AddMaterialConfigButton;
        private Button SaveConfigToListButton;

        private VisualElement F2MConfigGroup;
        private VisualElement R2MConfigGroup;
        private VisualElement EnemyConfigGroup;
        private VisualElement PlayerConfigGroup;


        private FloatField GridSpawnMaxChanceField;
        private FloatField GridSpawnMinChanceField;

        private IntegerField R2MSpawnAmountMaxChanceField;
        private IntegerField R2MSpawnAmountMinChanceField;

        private VisualElement ConfigItemListView;

        private ObjectField EnemyDataField;

        private List<F2MConfigItem> f2mConfigItemList;

        private float gridSpawnMaxChance;
        private float gridSpawnMinChance;

        private int r2MSpawnAmountMax;
        private int r2MSpawnAmountMin;

        private EnemyBaseData enemyData;


        private void InitF2MConfigSpace()
        {
            SelectFurnitureLabel = root.Q<Label>(nameof(SelectFurnitureLabel));
            FurnitureGridLabel = root.Q<Label>(nameof(FurnitureGridLabel));

            AddMaterialConfigButton = root.Q<Button>(nameof(AddMaterialConfigButton));
            AddMaterialConfigButton.clicked += OnAddMaterialConfigButtonClicked;

            SaveConfigToListButton = root.Q<Button>(nameof(SaveConfigToListButton));
            SaveConfigToListButton.clicked += OnSaveConfigToListButtonClicked;

            GridSpawnMaxChanceField = root.Q<FloatField>(nameof(GridSpawnMaxChanceField));
            GridSpawnMaxChanceField.RegisterValueChangedCallback(GridSpawnMaxChanceFieldValueChanged);
            GridSpawnMaxChanceField.value = 0.8f;
            gridSpawnMaxChance = 0.8f;

            GridSpawnMinChanceField = root.Q<FloatField>(nameof(GridSpawnMinChanceField));
            GridSpawnMinChanceField.RegisterValueChangedCallback(GridSpawnMinChanceFieldValueChanged);
            GridSpawnMinChanceField.value = 0.2f;
            gridSpawnMinChance = 0.2f;

            R2MSpawnAmountMaxChanceField = root.Q<IntegerField>(nameof(R2MSpawnAmountMaxChanceField));
            R2MSpawnAmountMaxChanceField.RegisterValueChangedCallback(R2MSpawnAmountMaxChanceFieldValueChanged);
            R2MSpawnAmountMaxChanceField.value = 5;
            r2MSpawnAmountMax = 5;

            R2MSpawnAmountMinChanceField = root.Q<IntegerField>(nameof(R2MSpawnAmountMinChanceField));
            R2MSpawnAmountMinChanceField.RegisterValueChangedCallback(R2MSpawnAmountMinChanceFieldValueChanged);
            R2MSpawnAmountMinChanceField.value = 1;
            r2MSpawnAmountMin = 1;

            EnemyDataField = root.Q<ObjectField>(nameof(EnemyDataField));
            EnemyDataField.objectType = typeof(EnemyBaseData);
            EnemyDataField.RegisterValueChangedCallback(EnemyDataFieldValueChanged);


            F2MConfigGroup = root.Q<VisualElement>(nameof(F2MConfigGroup));
            R2MConfigGroup = root.Q<VisualElement>(nameof(R2MConfigGroup));
            EnemyConfigGroup = root.Q<VisualElement>(nameof(EnemyConfigGroup));
            PlayerConfigGroup = root.Q<VisualElement>(nameof(PlayerConfigGroup));


            ConfigItemListView = root.Q<VisualElement>(nameof(ConfigItemListView));

            f2mConfigItemList = new List<F2MConfigItem>();
        }



        private void SetConfigMode(LevelConfigModeType mode)
        {
            currentMode = mode;
            R2MConfigGroup.style.display = currentMode == LevelConfigModeType.R2M ? DisplayStyle.Flex : DisplayStyle.None;
            F2MConfigGroup.style.display = currentMode == LevelConfigModeType.F2M ? DisplayStyle.Flex : DisplayStyle.None;
            EnemyConfigGroup.style.display = currentMode == LevelConfigModeType.Enemy ? DisplayStyle.Flex : DisplayStyle.None;
            PlayerConfigGroup.style.display = currentMode == LevelConfigModeType.Player ? DisplayStyle.Flex : DisplayStyle.None;


            if(currentMode != LevelConfigModeType.R2M)
            {
                foreach (var item in editorItemList)
                {
                    item.UnSelect();
                }

                if(currentMode == LevelConfigModeType.F2M)
                    curSelectItem = null;
            }
        }

        private void UpdateF2MConfigView()
        {
            if (gameLevelData == null) return;
            if (curMapFurnitureData == null) return;

            SelectFurnitureLabel.text = "��ǰѡ�еļҾߣ�" + curMapFurnitureData.furnitureData.furnitureName;
            FurnitureGridLabel.text = "�üҾߵĸ������Ų���" + curMapFurnitureData.furnitureData.gridLayout.x + "��" + curMapFurnitureData.furnitureData.gridLayout.y;

            //���ɸüҾ���ԭ�е�����

            Furniture2MaterialMapping mapping = gameLevelData.f2MMappingList.Find(x => x.serialNumber == curMapFurnitureData.serialNumber);
            foreach (var item in f2mConfigItemList)
            {
                item.Destory();
            }
            f2mConfigItemList.Clear();
            if (mapping != null)
            {
                GridSpawnMaxChanceField.value = mapping.gridSpawnMatChance_max;
                GridSpawnMinChanceField.value = mapping.gridSpawnMatChance_min;
                foreach (var cfg in mapping.materialDataList)
                {
                    F2MConfigItem configItem = new F2MConfigItem();
                    f2mConfigItemList.Add(configItem);
                    configItem.Init(ConfigItemListView, () =>
                    {
                        f2mConfigItemList.Remove(configItem);
                    });
                    configItem.SetInfo(cfg.materialId, cfg.randomAmount_max, cfg.randomAmount_min, cfg.spawnChance);
                }
            }
        }

        private void OnSaveConfigToListButtonClicked()
        {
            if (f2mConfigItemList == null) return;
            if (curMapFurnitureData == null) return;
            Furniture2MaterialMapping f2MMapping = gameLevelData.f2MMappingList.Find(x => x.serialNumber == curMapFurnitureData.serialNumber);
            //�б�ԭ�������� ��������
            if (f2MMapping != null)
            {
                if(f2mConfigItemList.Count == 0)
                {
                    gameLevelData.f2MMappingList.Remove(f2MMapping);
                    return;
                }
                f2MMapping.gridSpawnMatChance_min = gridSpawnMinChance;
                f2MMapping.gridSpawnMatChance_max = gridSpawnMaxChance;
                f2MMapping.materialDataList.Clear();
                foreach (var cfgItem in f2mConfigItemList)
                {
                    f2MMapping.materialDataList.Add(new MaterialResCfg(cfgItem.MaterialID, cfgItem.MaterialAmountMin, cfgItem.MaterialAmountMax, cfgItem.MaterialSpawnChance));
                }
            }
            else
            {
                if (f2mConfigItemList.Count == 0)
                {
                    Debug.LogWarning("û�������κ����ɵĲ��ϣ���Ӳ������õ��б�");
                    return;
                }
                f2MMapping = new Furniture2MaterialMapping();
                f2MMapping.serialNumber = curMapFurnitureData.serialNumber;
                f2MMapping.gridSpawnMatChance_min = gridSpawnMinChance;
                f2MMapping.gridSpawnMatChance_max = gridSpawnMaxChance;
                f2MMapping.materialDataList = new List<MaterialResCfg>();
                foreach (var cfgItem in f2mConfigItemList)
                {
                    f2MMapping.materialDataList.Add(new MaterialResCfg(cfgItem.MaterialID, cfgItem.MaterialAmountMin, cfgItem.MaterialAmountMax, cfgItem.MaterialSpawnChance));
                }
                gameLevelData.f2MMappingList.Add(f2MMapping);
            }
        }

        private void OnAddMaterialConfigButtonClicked()
        {
            if (f2mConfigItemList == null) return;
            if (curMapFurnitureData == null) return;

            F2MConfigItem configItem = new F2MConfigItem();
            f2mConfigItemList.Add(configItem);
            configItem.Init(ConfigItemListView, ()=>
            {
                f2mConfigItemList.Remove(configItem);
            });
        }

        private void GridSpawnMaxChanceFieldValueChanged(ChangeEvent<float> evt)
        {
            gridSpawnMaxChance = evt.newValue;
        }
        private void GridSpawnMinChanceFieldValueChanged(ChangeEvent<float> evt)
        {
            gridSpawnMinChance = evt.newValue;
        }

        private void R2MSpawnAmountMaxChanceFieldValueChanged(ChangeEvent<int> evt)
        {
            r2MSpawnAmountMax = evt.newValue;
        }

        private void R2MSpawnAmountMinChanceFieldValueChanged(ChangeEvent<int> evt)
        {
            r2MSpawnAmountMin = evt.newValue;
        }

        private void EnemyDataFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            enemyData = evt.newValue as EnemyBaseData;
        }
        #endregion
    }

    public enum ResConfigMenuType
    {
        Material = 0,
        Logic = 1,//�߼������ã����������λ��
    }

    public enum LevelConfigModeType
    {
        R2M,
        F2M,
        Enemy,
        Player,
    }


    public class LevelResMapEditorConfig
    {
        public const int standardGridUnitLength = 40; // ��׼����λ�߳�
        public const int maxGridUnitLength = 60; //  �������λ�߳�
        public const int minGridUnitLength = 20; //  ��С����λ�߳�
        public const int maxMapSizeX = 900;
        public const int maxMapSizeY = 600;
        public int curGridUnitLength = 40; //  ��ǰ����λ�߳�
    }

}
