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

        public Color unSelectColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
        public Color selectColor = Color.yellow;

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

        }

        public void ResetView()
        {

        }


        #region TopMenu
        private Button InfoButton;
        private ObjectField LevelResDataField; //关卡资源数据配置
        private ObjectField MapDataField; //关卡资源数据配置

        private LevelResData levelResData;//当前的关卡资源数据
        private MapData mapData;

        private void InitTopMenu()
        {
            InfoButton = root.Q<Button>(nameof(InfoButton));
            InfoButton.clicked += OnInfoButtonClicked;

            LevelResDataField = root.Q<ObjectField>(nameof(LevelResDataField));
            LevelResDataField.objectType = typeof(LevelResData);
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
            levelResData = evt.newValue as LevelResData;
            UpdateWorkSpaceView();
        }
        private void OnLevelResDataFieldClicked(MouseDownEvent evt)
        {
            if (levelResData != null)
            {
                Selection.activeObject = levelResData; //设置Inspector显示
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
            if (levelResData != null)
            {
                EditorUtility.SetDirty(levelResData);
                AssetDatabase.SaveAssetIfDirty(levelResData);
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
        private ItemMenuType itemMenuType;
        private int itemMenuIdx;
        private void InitItemMenu()
        {
            ItemListView = root.Q<VisualElement>(nameof(ItemListView));
            LeftButton = root.Q<Button>(nameof(LeftButton));
            LeftButton.clicked += OnLeftButtonClicked;

            RightButton = root.Q<Button>(nameof(RightButton));
            RightButton.clicked += OnRightButtonClicked;

            ItemGroupTitle = root.Q<Label>(nameof(ItemGroupTitle));


;
        }

        private void OnRightButtonClicked()
        {
            throw new NotImplementedException();
        }

        private void OnLeftButtonClicked()
        {
            throw new NotImplementedException();
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
        public void SetButtonBorderColor(Button btn, Color color)
        {
            btn.style.borderBottomColor = color;
            btn.style.borderTopColor = color;
            btn.style.borderLeftColor = color;
            btn.style.borderRightColor = color;
        }


        #endregion

        #region WorkSpace

        private void UpdateWorkSpaceView()
        {

        }
        #endregion
    }

    public enum ResConfigMenuType
    {
        Material = 0,
        Logic = 1,//逻辑点配置，如敌人生成位置
    }
}
