using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using KidGame.UI.Game;
using UnityEngine;


namespace KidGame.Core 
{
    /// <summary>
    /// 通用的可以拿东西的家具
    /// </summary>
    public class MapFurniture_CommonContainer : MapFurniture, IInteractive
    {

        public virtual void InteractPositive(GameObject interactor)
        {
            if(GameManager.Instance.IsGamePaused)return;
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("空空如也", transform.position));
            }
            else
            {
                UIController.Instance.ShowPopItemContainerWindow(materialHoldList,mapFurnitureData.furnitureData.gridLayout.x,mapFurnitureData.furnitureData.gridLayout.y);
            }
        }

        public void InteractNegative(GameObject interactor)
        {
            
        }
    }
}
