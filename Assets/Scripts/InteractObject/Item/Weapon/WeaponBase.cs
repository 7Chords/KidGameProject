using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KidGame.Core
{

    public abstract class WeaponBase : MapItem, IInteractive
    {
        private bool isOnHand = true;
        // �ٶ�
        [SerializeField] protected float speed = 0f;
        // ���ݶ�ȥ����ð�
        private WeaponData _weaponData;
        // �Լ�������
        protected GameObject self;
        
        // ��������
        protected LineRenderer lineRenderer;

        // �ű� ���ڸ�ֵ
        protected LineRenderScript lineRenderScript;

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
            // ���ɲ��������� ����ֻ���߼�
            self = this.gameObject;
            InitLineRender();
        }

        protected virtual void Update()
        {
            if(!isOnHand) WeaponUseLogic();
        }
        public virtual void InitWeaponData(WeaponData weaponData)
        {
            _weaponData = weaponData;
        }
        protected virtual void InitLineRender()
        {
            // �����߽ű�����
            lineRenderer = this.AddComponent<LineRenderer>();
            // �ѿ������ߵĽű�����
            lineRenderScript = this.AddComponent<LineRenderScript>();
            lineRenderScript.lineRenderer = lineRenderer;
            lineRenderScript.startPoint = this.transform.position;
            lineRenderScript.endPoint = MouseRaycaster.Instance.GetMousePosi();
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
            PlayerUtil.Instance.CallPlayerPickItem(weaponData.id,UseItemType.weapon);
            //�������Ҳ����������ص��ӵ��б�����   ������ط��Ƴ�
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

            //TODO:�������������ܻ���ʲôҪ�����Ķ��� ����

            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", gameObject));

            Destroy(gameObject);
        }

        public void SetOnHandOrNot(bool onHand)
        {
            isOnHand = onHand;
        }
        public bool GetOnHandOrNot()
        {
            return isOnHand;
        }
        public virtual void InteractPositive(GameObject interactor)
        {
            //To Do:�������ܻᱻ������ħ
        }
 
        public virtual void InteractNegative(GameObject interactor)
        {

        }

        public abstract void WeaponUseLogic();
    }

}
