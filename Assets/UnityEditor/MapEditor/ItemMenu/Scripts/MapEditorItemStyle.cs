using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditorItemStyle
{
    private const string mapEditorItemAssetPath = "Assets/UnityEditor/MapEditor/ItemMenu/Assets/Item.uxml";
    private VisualElement selfParent;
    private VisualElement selfRoot;
    public VisualElement SelfRoot => selfRoot;

    public void Init(VisualElement parent,string name)
    {
        selfParent = parent;
        selfRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(mapEditorItemAssetPath).Instantiate().Query().ToList()[1];
        parent.Add(selfRoot);
        ((Button)selfRoot).text = name;
    }

    public void Destory()
    {
        if(selfRoot!=null)
        {
            selfParent.Remove(selfRoot);
        }
    }

    //public void Select()
    //{
    //}
    //public void UnSelect()
    //{
    //}
}
