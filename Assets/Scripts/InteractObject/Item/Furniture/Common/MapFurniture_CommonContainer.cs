using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using KidGame.UI.Game;
using UnityEngine;


namespace KidGame.Core 
{
    /// <summary>
    /// ͨ�õĿ����ö����ļҾ�
    /// </summary>
    public class MapFurniture_CommonContainer : MapFurniture, IInteractive
    {

        public virtual void InteractPositive(GameObject interactor)
        {
            if(GameManager.Instance.IsGamePaused)return;
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("�տ���Ҳ", gameObject));
            }
            else
            {
                UIController.Instance.ShowPopItemContainerWindow(materialHoldList,mapFurnitureData.furnitureData.gridLayout.x,mapFurnitureData.furnitureData.gridLayout.y);
                /*
                foreach (var item in materialHoldList)
                {
                    for (int i = 0; i < item.amount; i++)
                    {
                        //ԭ����
                        PlayerUtil.Instance.CallPlayerPickItem(item.data.id,UseItemType.Material);
                        UIHelper.Instance.ShowTipByQueue(new TipInfo("�����" + item.data.materialName + "��1", gameObject, 0.5f));
                    }
                }*/
        
                //materialHoldList.Clear();
            }
        }

        public void InteractNegative(GameObject interactor)
        {
            
        }
    }
}
