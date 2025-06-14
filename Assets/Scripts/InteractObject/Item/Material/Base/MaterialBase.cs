using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ϻ��� ����Ŀǰ����ֻ�б��������������
/// </summary>
public class MaterialBase : MapItem
{
    //����public
    public MaterialData _materialData;

    public MaterialData materialData => _materialData;
    public override void InteractNegative()
    {
    }

    public override void InteractPositive()
    {
    }

    public override void Pick()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }
}
