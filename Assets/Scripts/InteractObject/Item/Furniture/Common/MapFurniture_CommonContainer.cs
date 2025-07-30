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
    public class MapFurniture_CommonContainer : MapFurniture, IPickable
    {

        public virtual void Pick()
        {
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("�տ���Ҳ", gameObject));
            }
            else
            {
                foreach (var item in materialHoldList)
                {
                    for (int i = 0; i < item.amount; i++)
                    {
                        UIController.Instance.ShowPopItemContainerWindow();



                        //ԭ����
                        /*PlayerUtil.Instance.CallPlayerPickItem(item.data.id,UseItemType.Material);
                        UIHelper.Instance.ShowTipByQueue(new TipInfo("�����" + item.data.materialName + "��1", gameObject, 0.5f));*/
                    }
                }
                materialHoldList.Clear();
            }
        }
    }
}
