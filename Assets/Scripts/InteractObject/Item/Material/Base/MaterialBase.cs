using KidGame.Core;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ���ϻ��� ����Ŀǰ����ֻ�б��������������
    /// </summary>
    public class MaterialBase : MapItem
    {
        //����public
        public MaterialData _materialData;

        public MaterialData materialData => _materialData;

        public override string itemName => _materialData.materialName;

        /// <summary>
        /// ��ʼ��
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
            BubbleManager.Instance.RemoveBubbleInfoFromList(gameObject);
            //TODO:�������գ�
            Destroy(gameObject);
        }

    }
}
