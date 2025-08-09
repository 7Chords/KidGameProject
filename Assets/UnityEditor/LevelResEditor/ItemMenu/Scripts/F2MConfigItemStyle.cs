using UnityEditor;
using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class F2MConfigItemStyle 
    {
        private const string levelEditorF2MConfigItemAssetPath = "Assets/UnityEditor/LevelResEditor/ItemMenu/Assets/F2MConfigItem.uxml";
        private VisualElement selfParent;
        private VisualElement selfRoot;
        public VisualElement SelfRoot => selfRoot;

        public void Init(VisualElement parent)
        {
            selfParent = parent;
            selfRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(levelEditorF2MConfigItemAssetPath).Instantiate().Query()
                .ToList()[1];
            parent.Add(selfRoot);
        }
        public void Destory()
        {
            if (selfRoot != null)
            {
                selfParent.Remove(selfRoot);
            }
        }
    }
}
