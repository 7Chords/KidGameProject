using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.Editor
{
    /// <summary>
    /// 关卡资源配置器编辑器窗口
    /// </summary>
    public class LevelResEditorWindow : EditorWindow
    {
        public static LevelResEditorWindow Instance;


        [MenuItem("自定义编辑器/关卡资源配置器")]
        public static void ShowExample()
        {
            LevelResEditorWindow wnd = GetWindow<LevelResEditorWindow>();
            wnd.titleContent = new GUIContent("关卡资源配置器");
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

        }
    }
}
