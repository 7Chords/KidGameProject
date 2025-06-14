using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 材料基类 但是目前材料只有被捡起来这个功能
/// </summary>
public class MaterialBase : MapItem
{
    //测试public
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
