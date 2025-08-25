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

        public override string EntityName => _materialData.materialName;

        public int playerGetAmount;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="materialData"></param>
        public virtual void Init(MaterialData materialData,int getAmount)
        {
            _materialData = materialData;
            playerGetAmount = getAmount;
        }

        public override void Pick()
        {
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.ShowOneTip(new TipInfo("获得了" + EntityName + "×" + playerGetAmount, transform.position));
            for (int i =0;i<playerGetAmount;i++)
            {
                MsgCenter.SendMsg(MsgConst.ON_PICK_ITEM, _materialData.id, UseItemType.Material);
            }

            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
            //播放音效和特效
            if (!string.IsNullOrEmpty(materialData.pickSoundName))
            {
                AudioManager.Instance.PlaySfx(materialData.pickSoundName);
            }

            if (!string.IsNullOrEmpty(materialData.pickParticleName))
            {
                ParticleManager.Instance.PlayEffect(materialData.pickParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
                    1f);
            }
            //TODO:工厂回收？
            Destroy(gameObject);
        }
    }
}