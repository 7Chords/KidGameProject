using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  �����еĿ�ʰȡ��Ʒ
/// </summary>
public abstract class MapItem : MonoBehaviour, IPickable,IInteractive
{
    /// <summary>
    /// ����
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// ʰȡ
    /// </summary>
    public abstract void Pick();
}