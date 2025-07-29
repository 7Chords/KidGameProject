using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.Editor
{
    /// <summary>
    /// �ؿ���Դ�������༭������
    /// </summary>
    public class LevelResEditorWindow : EditorWindow
    {
        public static LevelResEditorWindow Instance;


        [MenuItem("�Զ���༭��/�ؿ���Դ������")]
        public static void ShowExample()
        {
            LevelResEditorWindow wnd = GetWindow<LevelResEditorWindow>();
            wnd.titleContent = new GUIContent("�ؿ���Դ������");
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
