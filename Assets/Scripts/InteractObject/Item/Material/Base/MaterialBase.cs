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


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="materialData"></param>
        public virtual void Init(MaterialData materialData)
        {
            _materialData = materialData;
        }
        public override void InteractPositive()
        {
            if (_materialData == null) return;
            Pick();
        }
        public override void InteractNegative()
        {
        }

        public override void Pick()
        {
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerUtil.Instance.CallPlayerPickItem(this);
            BubbleManager.Instance.RemoveBubbleInfoFromList(new BubbleInfo(ControlType.Keyborad, null,
    gameObject, null, ""));
            //TODO:工厂回收？
            Destroy(gameObject);
        }

    }
}
