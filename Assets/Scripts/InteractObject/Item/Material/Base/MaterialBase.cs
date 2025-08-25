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

        public int playerGetAmount;
        /// <summary>
        /// ��ʼ��
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
            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��" + playerGetAmount, transform.position));
            for (int i =0;i<playerGetAmount;i++)
            {
                MsgCenter.SendMsg(MsgConst.ON_PICK_ITEM, _materialData.id, UseItemType.Material);
            }

            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
            //������Ч����Ч
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
            //TODO:�������գ�
            Destroy(gameObject);
        }
    }
}