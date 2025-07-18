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
            //todo:播放音效和特效
            if (!string.IsNullOrEmpty(materialData.pickSoundName))
            {
                AudioManager.Instance.PlaySfx(materialData.pickSoundName);
            }

            if (!string.IsNullOrEmpty(materialData.pickParticleName))
            {
                ParticleManager.Instance.PlayEffect(materialData.pickParticleName,
                    transform.position,
                    Quaternion.identity,
                    transform,
                    true,
                    1f);
            }
            UIHelper.Instance.ShowOneTip(new TipInfo("获得了" + EntityName + "×1", gameObject));
            //TODO:工厂回收？
            Destroy(gameObject);
        }
    }
}