using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditorItem
{
    private MapEditorItemStyle itemStyle;

    private string itemName;
    public string ItemName => itemName;
    public void Init(VisualElement parent,string name)
    {
        itemName = name;

        itemStyle = new MapEditorItemStyle();
        itemStyle.Init(parent, name);

        ((Button)itemStyle.SelfRoot).clicked += OnItemButtonClicked;
    }

    private void OnItemButtonClicked()
    {
        MapEditorWindow.Instance.SelectOneItem(this);
    }


    public void Select()
    {
        MapEditorWindow.Instance.SetButtonBorderColor(((Button)itemStyle.SelfRoot), MapEditorWindow.Instance.selectColor);
    }

    public void UnSelect()
    {
        MapEditorWindow.Instance.SetButtonBorderColor(((Button)itemStyle.SelfRoot), MapEditorWindow.Instance.unSelectColor);
    }

    public void Destory()
    {
        if(itemStyle!=null)
        {
            itemStyle.Destory();
        }
    }
}
