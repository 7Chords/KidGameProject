using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  �����еĿ�ʰȡ��Ʒ
/// </summary>
public class MapItem : MonoBehaviour, IPickable
{
    public string itemName;

    public void Deal()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }
}