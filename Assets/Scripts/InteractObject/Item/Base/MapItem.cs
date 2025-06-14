using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  场景中的可拾取物品
/// </summary>
public abstract class MapItem : MonoBehaviour, IPickable,IInteractive
{
    /// <summary>
    /// 主动交互
    /// </summary>
    public abstract void InteractPositive();

    /// <summary>
    /// 被动交互
    /// </summary>
    public abstract void InteractNegative();

    /// <summary>
    /// 拾取
    /// </summary>
    public abstract void Pick();
}