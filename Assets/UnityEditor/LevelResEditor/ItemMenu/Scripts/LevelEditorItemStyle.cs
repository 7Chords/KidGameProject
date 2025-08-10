using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class LevelEditorItemStyle
    {
        private const string mapEditorItemAssetPath = "Assets/UnityEditor/LevelResEditor/ItemMenu/Assets/Item.uxml";
        private VisualElement selfParent;
        private VisualElement selfRoot;
        public VisualElement SelfRoot => selfRoot;

        private Label RareLabel;

        public void Init(VisualElement parent, string name,int rare)
        {
            selfParent = parent;
            selfRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(mapEditorItemAssetPath).Instantiate().Query()
                .ToList()[1];
            RareLabel = selfRoot.Q<Label>(nameof(RareLabel));
            parent.Add(selfRoot);

            ((Button)selfRoot).text = name;
            RareLabel.text = rare.ToString();
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