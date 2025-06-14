using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrapSlotInfo
{
    public string trapID;
    public string trapName;
    public string trapDesc;
    public Sprite trapIcon;
    public int amount;

    public TrapSlotInfo(string trapID, string trapName, string trapDesc, Sprite trapIcon, int amount)
    {
        this.trapID = trapID;
        this.trapName = trapName;
        this.trapDesc = trapDesc;
        this.trapIcon = trapIcon;
        this.amount = amount;
    }
}

[Serializable]
public class MaterialSlotInfo
{
    public string materialID;
    public string materialName;
    public string materialDesc;
    public Sprite materialIcon;
    public int amount;

    public MaterialSlotInfo(string materialID, string materialName, string materialDesc, Sprite materialIcon, int amount)
    {
        this.materialID = materialID;
        this.materialName = materialName;
        this.materialDesc = materialDesc;
        this.materialIcon = materialIcon;
        this.amount = amount;
    }
}

public class PlayerBag : Singleton<PlayerBag>
{
    #region 物品列表

    // 当前捡到的物品、以及从存档中读取数据、保存数据的逻辑
    public List<TrapSlotInfo> _trapBag;
    public List<MaterialSlotInfo> _materialBag;

    #endregion

    #region 注册事件

    public void Init()
    {
        PlayerUtil.Instance.RegPlayerPickItem(PlayerInteractItem);

        _trapBag = new List<TrapSlotInfo>();
        _materialBag = new List<MaterialSlotInfo>();
    }

    public void Discard()
    {
        PlayerUtil.Instance.UnregPlayerPickItem(PlayerInteractItem);
    }

    #endregion


    private void PlayerInteractItem(IPickable iPickable)
    {
        if (iPickable == null) return;
        if (iPickable is TrapBase)
        {
            TrapData trapData = (iPickable as TrapBase).trapData;
            if (trapData == null) return;
            TrapSlotInfo trapSlotInfo = _trapBag.Find(x => x.trapID == trapData.trapID);
            if(trapSlotInfo == null)
            {
                _trapBag.Add(new TrapSlotInfo(trapData.trapID,trapData.trapName,trapData.trapDesc,trapData.trapIcon, 1));
            }
            else
            {
                trapSlotInfo.amount++;
            }

        }
        else if(iPickable is MaterialBase)
        {
            MaterialData materialData = (iPickable as MaterialBase).materialData;
            if (materialData == null) return;
            MaterialSlotInfo materialSlotInfo = _materialBag.Find(x => x.materialID == materialData.materialID);
            if (materialSlotInfo == null)
            {
                _materialBag.Add(new MaterialSlotInfo(materialData.materialID, materialData.materialName, materialData.materialDesc, materialData.materialIcon, 1));
            }
            else
            {
                materialSlotInfo.amount++;
            }
        }
    }



}