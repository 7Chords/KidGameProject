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
    public class MapFurniture_CommonContainer : MapFurniture, IInteractive
    {


        public void InteractPositive(GameObject interactor)
        {
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("空空如也", gameObject));
            }
            else
            {
                //foreach (var item in materialHoldList)
                //{
                //    for (int i = 0; i < item.amount; i++)
                //    {
                //        PlayerUtil.Instance.CallPlayerPickItem(item.data.id, UseItemType.Material);
                //        UIHelper.Instance.ShowTipByQueue(new TipInfo("获得了" + item.data.materialName + "×1", gameObject, 0.5f));
                //    }
                //}
                //materialHoldList.Clear();

                //todo：传入materialHoldList打开获取材料面板 rst
            }
        }
        public void InteractNegative(GameObject interactor) { }

    }
}
