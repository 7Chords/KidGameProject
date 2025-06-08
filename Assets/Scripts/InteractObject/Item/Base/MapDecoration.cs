using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景中的装饰物，不需要实现具体的逻辑
/// </summary>
public class MapDecoration : MonoBehaviour, IInteractive
{
    public string decoratinoName;

    public void Deal()
    {
        Debug.Log($"和{decoratinoName}互动了！");
    }
}