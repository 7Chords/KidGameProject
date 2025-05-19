using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/**
 * ��ͼ�༭�����ڽű�
 */
public class MapEditorWindow : EditorWindow
{
    public static MapEditorWindow Instance;

    private MapEditorConfig mapEditorConfig = new MapEditorConfig();

    [MenuItem("�Զ���༭��/��ͼ�༭��")]
    public static void ShowExample()
    {
        MapEditorWindow wnd = GetWindow<MapEditorWindow>();
        wnd.titleContent = new GUIContent("��ͼ�༭��");
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

    private void InitTopMenu()
    {
        EditRoomButton = root.Q<Button>(nameof(EditRoomButton));
        EditRoomButton.clicked += OnEditRoomButtonClicked;

        EditHouseButton = root.Q<Button>(nameof(EditHouseButton));
        EditHouseButton.clicked += OnEditHouseButtonClicked;

        InfoButton = root.Q<Button>(nameof(InfoButton));
        InfoButton.clicked += OnInfoButtonClicked;
    }

    private void OnEditRoomButtonClicked()
    {
        Debug.Log("EditRoomButtonClick");
    }

    private void OnEditHouseButtonClicked()
    {
        Debug.Log("EditHouseButtonClick");
    }

    private void OnInfoButtonClicked()
    {
        Debug.Log("InfoButtonClick");
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
        //IMGUI�Ļ��ƺ���
        WorkContainer.onGUIHandler = DrawWorkSpace;
        //WorkContainer.RegisterCallback<WheelEvent>(TimerShaftWheel);
        //WorkContainer.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
        //WorkContainer.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
        //WorkContainer.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
        //WorkContainer.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);
    }

    private void DrawWorkSpace()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;

        Rect rect = WorkContainer.contentRect;
        int row = (int)rect.width / MapEditorConfig.standardGridUnitLength;//��
        int column = (int)rect.height / MapEditorConfig.standardGridUnitLength;//��

        int curStartPos = 0;
        for(int i=0;i<row;i++)
        {
            Handles.DrawLine(new Vector3(0, curStartPos + i * MapEditorConfig.standardGridUnitLength),
                new Vector3(rect.width, curStartPos + i * MapEditorConfig.standardGridUnitLength));
            curStartPos += MapEditorConfig.standardGridUnitLength;
        }
        curStartPos = 0;
        for (int i = 0; i < column; i++)
        {
            Handles.DrawLine(new Vector3(curStartPos + i * MapEditorConfig.standardGridUnitLength, 0),
                new Vector3(curStartPos + i * MapEditorConfig.standardGridUnitLength, rect.width));
            curStartPos += MapEditorConfig.standardGridUnitLength;
        }

        //Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
        //string indexStr = index.ToString();
        //GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);




        //// ��ʼ����
        //int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
        //// �����������ƫ��
        //float startOffset = 0;
        //if (index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);

        ////������ٿ̶Ȼ�һ������
        //int tickStep = SkillEditorConfig.maxFrameWidthLV + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.standFrameUnitWidth);
        //tickStep = tickStep / 2; // ���� 1 / 2 = 0�����
        //if (tickStep == 0) tickStep = 1; // ����Ϊ0
        //for (float i = startOffset; i < rect.width; i += skillEditorConfig.frameUnitWidth)
        //{
        //    // ���Ƴ��ߡ��ı�
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

public class MapEditorConfig
{
    public const int standardGridUnitLength = 20;  // ��׼����λ�߳�
    public const int maxGridUnitLength = 40;      //  �������λ�߳�
    public const int minGridUnitLength = 10;      //  ��С����λ�߳�
    public int curGridUnitLength = 20;             //  ��ǰ����λ�߳�
}
