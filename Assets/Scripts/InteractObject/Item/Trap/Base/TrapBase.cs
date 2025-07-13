using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// �������
    /// </summary>
    public class TrapBase : MapItem, IInteractive, IStateMachineOwner,IMouseShowPreview
    {

        [SerializeField] private GameObject previewGO;
        public GameObject PreviewGO 
        {
            get => previewGO;
            set { previewGO = value; }
        }

        public override string EntityName => trapData.trapName;

        [ColorUsage(true, true)] public Color NoReadyColor;

        [ColorUsage(true, true)] public Color ReadyColor;

        public Renderer ReadyIndicator;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Collider coll;
        public Collider Coll => coll;


        protected TrapData trapData;
        public TrapData TrapData => trapData;


        protected GameObject interactor;
        public GameObject Interactor => interactor;

        protected StateMachine stateMachine;
        private TrapState trapState;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
            gameObject.AddComponent<MousePreviewDetector>();
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="trapData"></param>
        public virtual void Init(TrapData trapData)
        {
            this.trapData = trapData;

            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //��ʼ��ΪIdle״̬
            ChangeState(TrapState.NoReady);
        }

        public virtual void Discard()
        {
            //todo:fix
            //stateMachine.Destory();
            Destroy(gameObject);
        }


        #region �����ӿڷ���ʵ��

        public virtual void InteractPositive(GameObject interactor)
        {
            if (trapData == null) return;
            //��Ҫ��ý
            if (trapData.triggerType == TrapTriggerType.Negative) return;
            //�ж������Ƿ���Ч
            if (trapState != TrapState.Ready) return;
            //todo:������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.interactSoundPath))
            {
                AudioManager.Instance.PlaySfx(trapData.interactSoundPath);
            }

            if (!string.IsNullOrEmpty(trapData.interactParticalPath))
            {
                //todo:partical mgr?
                MonoManager.Instance.InstantiateGameObject(
                    Resources.Load<GameObject>(trapData.interactParticalPath),
                    transform.position, Quaternion.identity,
                    1f);
            }

            RemoveFormPlayerUsingList();
            this.interactor = interactor;
            ChangeState(TrapState.Running);
        }


        public virtual void InteractNegative( GameObject interactor)
        {
            if (trapData == null || trapData.triggerType != TrapTriggerType.Negative) return;
            //�ж������Ƿ���Ч
            if (trapState != TrapState.Ready) return;
            //todo:������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.interactSoundPath))
            {
                AudioManager.Instance.PlaySfx(trapData.interactSoundPath);
            }

            if (!string.IsNullOrEmpty(trapData.interactParticalPath))
            {
                //todo:partical mgr?
                MonoManager.Instance.InstantiateGameObject(
                    Resources.Load<GameObject>(trapData.interactParticalPath),
                    transform.position, Quaternion.identity,
                    1f);
            }

            RemoveFormPlayerUsingList();
            this.interactor = interactor;
            ChangeState(TrapState.Running);
        }

        public override void Pick()
        {
            RemoveFormPlayerUsingList();
            PlayerUtil.Instance.CallPlayerPickItem(this);
            //todo:������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.pickSoundPath))
            {
                AudioManager.Instance.PlaySfx(trapData.pickSoundPath);
            }

            if (!string.IsNullOrEmpty(trapData.pickParticalPath))
            {
                //todo:partical mgr?
                MonoManager.Instance.InstantiateGameObject(
                    Resources.Load<GameObject>(trapData.pickParticalPath),
                    transform.position, Quaternion.identity,
                    1f);
            }

            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", gameObject));
            ChangeState(TrapState.Dead);
        }

        #endregion

        #region ������

        /// <summary>
        /// ���崥����Ч������
        /// </summary>
        public virtual void Trigger()
        {
            Debug.Log(gameObject.name + "���崥����");
        }

        /// <summary>
        /// �ⲿ�¼�������������
        /// </summary>
        public virtual void DeadByExternal()
        {
            if (trapState == TrapState.Running &&
                trapData.deadTypeList.Contains(TrapDeadType.ExternalEvent))
            {
                ChangeState(TrapState.Dead);
            }
        }

        #endregion

        /// <summary>
        /// �޸�״̬
        /// </summary>
        public virtual void ChangeState(TrapState trapState, bool reCurrstate = false)
        {
            this.trapState = trapState;
            switch (trapState)
            {
                case TrapState.NoReady:
                    stateMachine.ChangeState<TrapNoReadyStateBase>((int)trapState, reCurrstate);
                    break;
                case TrapState.Ready:
                    stateMachine.ChangeState<TrapReadyStateBase>((int)trapState, reCurrstate);
                    break;
                case TrapState.Running:
                    stateMachine.ChangeState<TrapRunningStateBase>((int)trapState, reCurrstate);
                    break;
                case TrapState.Dead:
                    stateMachine.ChangeState<TrapDeadStateBase>((int)trapState, reCurrstate);
                    break;
                default:
                    break;
            }
        }


        private void RemoveFormPlayerUsingList()
        {
            coll.enabled = false;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
        }

        public void ShowPreview()
        {
            PreviewGO.SetActive(true);
        }
        public void HidePreview()
        {
            PreviewGO.SetActive(false);
        }
    }
}