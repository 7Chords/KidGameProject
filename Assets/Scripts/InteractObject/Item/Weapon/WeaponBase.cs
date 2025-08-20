/*using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KidGame.Core
{

    public abstract class WeaponBase : MapItem, IInteractive
    {
        protected bool isOnHand = true;
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
        }

        protected virtual void Update()
        {
            if (!isOnHand) WeaponUseLogic();
            else
            {
                SetStartPoint();
                SetEndPoint();
            }
        }

        // �����������յ�
        private void SetEndPoint()
        {
            if(lineRenderScript != null)
            {
                Vector3 curVector3 = MouseRaycaster.Instance.GetMousePosi();
                if(curVector3 != Vector3.zero)
                {
                    lineRenderScript.endPoint = curVector3;
                }
            }
        }
        // �������������
        private void SetStartPoint()
        {
            if (lineRenderScript != null)
            {
               lineRenderScript.startPoint = this.transform.position;
            }
        }
        public virtual void InitWeaponData(WeaponData weaponData)
        {
            _weaponData = weaponData;
        }
        protected virtual void InitLineRender()
        {
            // �����߽ű�����
            lineRenderer = GetComponent<LineRenderer>();
            // �ѿ������ߵĽű�����
            lineRenderScript = GetComponent<LineRenderScript>();
            lineRenderScript.lineRenderer = lineRenderer;
            *//*lineRenderScript.startPoint = 
                PlayerController.Instance.transform.position
                + PlayerController.Instance.transform.forward;*//*
            if(PlayerController.Instance != null)
            {
                lineRenderScript.startPoint = PlayerController.Instance.PlaceTrapPoint.position;
            }
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
*/


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
        protected bool isOnHand = true;
        protected WeaponData _weaponData;
        protected GameObject self;
        protected LineRenderer lineRenderer;
        protected LineRenderScript lineRenderScript;

        public override string EntityName { get => _weaponData.name; }
        public WeaponData weaponData => _weaponData;

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            self = gameObject;
            InitLineRender();
            //if(_weaponData != null && _weaponData.weaponType == 1) InitLineRender(); // �����Զ�̵��߲���Ҫ ��ʼ��������Ⱦ
        }

        protected virtual void Update()
        {
            WeaponUseLogic();
        }

        // �����������յ㣨���λ�ã�
        protected void SetEndPoint()
        {
            if (lineRenderScript != null)
            {
                Vector3 mousePos = MouseRaycaster.Instance.GetMousePosi();
                if (mousePos != Vector3.zero)
                    lineRenderScript.endPoint = mousePos;
            }
        }

        // ������������㣨����λ�ã�
        protected void SetStartPoint()
        {
            if (lineRenderScript != null)
            {
                if (PlayerController.Instance != null)
                    lineRenderScript.startPoint = PlayerController.Instance.PlaceTrapPoint.position;
                else
                    lineRenderScript.startPoint = transform.position;
            }
        }

        public virtual void InitWeaponData(WeaponData weaponData)
        {
            _weaponData = weaponData;
        }

        protected virtual void InitLineRender()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderScript = GetComponent<LineRenderScript>();
            if (lineRenderScript != null && lineRenderer != null)
                lineRenderScript.lineRenderer = lineRenderer;

            // ��ʼ�������յ�
            if (PlayerController.Instance != null)
                lineRenderScript.startPoint = PlayerController.Instance.PlaceTrapPoint.position;
            lineRenderScript.endPoint = MouseRaycaster.Instance.GetMousePosi();
        }

        // ���������߼�
        public override void Pick()
        {
            PlayerController.Instance.RemovePickableFromList(this);
            PlayerUtil.Instance.CallPlayerPickItem(weaponData.id, UseItemType.weapon);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", gameObject));
            Destroy(gameObject);
        }

        public void SetOnHandOrNot(bool onHand)
        {
            isOnHand = onHand;
            // ��������ع켣Ԥ��
            if (!onHand && lineRenderer != null)
                lineRenderer.enabled = false;
        }

        public bool GetOnHandOrNot() => isOnHand;

        public virtual void InteractPositive(GameObject interactor) { }
        public virtual void InteractNegative(GameObject interactor) { }

        public abstract void WeaponUseLogic();
    }
}