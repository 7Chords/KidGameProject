using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core 
{
    /// <summary>
    /// 通用的可以拿东西的家具
    /// </summary>
    public class MapFurniture_CommonContainer : MapFurniture, IPickable
    {

        public virtual void Pick()
        {
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("空空如也", gameObject));
            }
            else
            {
                MaterialBase tmpMat = new MaterialBase();
                foreach (var item in materialHoldList)
                {
                    for (int i = 0; i < item.amount; i++)
                    {
                        tmpMat.Init(item.data);
                        PlayerUtil.Instance.CallPlayerPickItem(tmpMat.materialData.id,UseItemType.Material);
                        UIHelper.Instance.ShowTipByQueue(new TipInfo("获得了" + item.data.materialName + "×1", gameObject, 0.5f));
                    }
                }
                materialHoldList.Clear();
            }
        }
    }
}
