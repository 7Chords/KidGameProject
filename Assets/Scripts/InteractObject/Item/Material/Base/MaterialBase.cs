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

        public override string EntityName => _materialData.materialName;

        /// <summary>
        /// ��ʼ��
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
            //todo:������Ч����Ч
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
            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", gameObject));
            //TODO:�������գ�
            Destroy(gameObject);
        }
    }
}