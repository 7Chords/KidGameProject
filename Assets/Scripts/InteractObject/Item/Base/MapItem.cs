using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  场景中的可拾取物品
/// </summary>
public class MapItem : MonoBehaviour, IPickable
{
    public string itemName;

    public void Deal()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }
}