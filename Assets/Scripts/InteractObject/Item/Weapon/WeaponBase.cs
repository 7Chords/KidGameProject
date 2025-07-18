using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{

    public class WeaponBase : MapItem, IInteractive
    {
        //���ݶ�ȥ����ð�
        private WeaponData _weaponData;
        public override string EntityName { get => _weaponData.name;}
        public WeaponData weaponData
        {
            get
            {
                return _weaponData;
            }
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            //To do:���ɵ�ʱ��Ҫճ�ڽ�ɫ����
        }

        protected virtual void Update()
        {
            
        }
        protected virtual void InitWeaponData(string id, string dataName, string soName)
        {
            _weaponData = SoLoader.Instance.GetDataById(id, dataName, soName) as WeaponData;
        }
        /// <summary>
        /// ���������ʱ�����
        /// </summary>
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

            Destroy(gameObject);
        }

        public virtual void InteractPositive(GameObject interactor)
        {
            //To Do:�������ܻᱻ������ħ
        }

        //To Do:����Ӧ�ò���Ҫ�������� ����  
        public virtual void InteractNegative(GameObject interactor)
        {

        }
    }

}
