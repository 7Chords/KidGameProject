using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class WeaponBase : MapItem, IInteractive
    {
        /// <summary>
        /// ���������ʱ�����
        /// </summary>

        private WeaponEntity _weaponData;

        public WeaponEntity weaponData
        {
            get
            {
                return _weaponData;
            }
        }

        public override void Pick()
        {
            //����ͨ��Trigger����Colider�����������
            //�����������ʱ����Ƴ�
            PlayerController.Instance.RemovePickableFromList(this);
            //Ŀǰ����ص�ֻע����һ�������Ʒ�������ĺ���
            PlayerUtil.Instance.CallPlayerPickItem(this);
            //�������Ҳ����������ص��ӵ��б�����   ������ط��Ƴ�
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

            //TODO:�������������ܻ���ʲôҪ�����Ķ��� ����


            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", gameObject));

        }

        public virtual void InteractPositive(GameObject interactor)
        {
            //���าд���Խ������� ����use����� ������
        }

        //To Do:����Ӧ�ò���Ҫ�������� ����  
        public virtual void InteractNegative(GameObject interactor)
        {

        }
    }

}
