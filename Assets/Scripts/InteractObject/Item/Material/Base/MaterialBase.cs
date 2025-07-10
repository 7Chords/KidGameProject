using KidGame.Core;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 材料基类 但是目前材料只有被捡起来这个功能
    /// </summary>
    public class MaterialBase : MapItem
    {
        //测试public
        public MaterialData _materialData;

        public MaterialData materialData => _materialData;

        public override string itemName => _materialData.materialName;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="materialData"></param>
        public virtual void Init(MaterialData materialData)
        {
            _materialData = materialData;
        }

        public override void Pick()
        {
            PlayerController.Instance.RemovePickableFromList(this);
            PlayerUtil.Instance.CallPlayerPickItem(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
            if (RandomPickSfxList != null && RandomPickSfxList.Count > 0)
            {
                AudioManager.Instance.PlaySfx(RandomPickSfxList[Random.Range(0, RandomPickSfxList.Count)]);
            }

            if (pickPartical != null)
            {
                MonoManager.Instance.InstantiateGameObject(pickPartical, transform.position, Quaternion.identity, 1f);
            }

            UIHelper.Instance.ShowTipImmediate(new TipInfo("获得了" + itemName + "×1", gameObject));
            //TODO:工厂回收？
            Destroy(gameObject);
        }
    }
}