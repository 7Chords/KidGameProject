using KidGame.Interface;
using KidGame.UI;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// �������
    /// </summary>
    public class TrapBase : MapItem, IInteractive, IStateMachineOwner,IMouseShowDetail
    {

        [SerializeField] private GameObject detailGO;
        public GameObject DetailGO 
        {
            get => detailGO;
            set { detailGO = value; }
        }

        [SerializeField] private GameObject previewGO;

        [SerializeField] private GameObject model;
        public override string EntityName => trapData.trapName;

        [ColorUsage(true, true)] public Color NoReadyColor;

        [ColorUsage(true, true)] public Color ReadyColor;

        [ColorUsage(true, true)] public Color CanPlaceColor;

        [ColorUsage(true, true)] public Color NoCanPlaceColor;


        public Renderer ReadyIndicator;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Collider coll;
        public Collider Coll => coll;

        //private NavMeshObstacle naveObstacle;

        //public NavMeshObstacle NaveObstacle => naveObstacle;

        protected TrapData trapData;
        public TrapData TrapData => trapData;


        protected GameObject interactor;
        public GameObject Interactor => interactor;

        protected StateMachine stateMachine;
        private TrapState trapState;

        private bool canPlaceTrap = true;
        public bool CanPlaceTrap => canPlaceTrap;

        private bool hasInited;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
            gameObject.AddComponent<MousePreviewDetector>();
            //naveObstacle = GetComponent<NavMeshObstacle>();
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

            //��������������
            coll.enabled = true;
            model.SetActive(true);
            hasInited = true;
        }

        public virtual void Discard()
        {
            //todo:fix
            //stateMachine.Destory();
            if (!string.IsNullOrEmpty(trapData.deadSoundName))
            {
                AudioManager.Instance.PlaySfx(trapData.deadSoundName);
            }

            if (!string.IsNullOrEmpty(trapData.deadParticleName))
            {
                ParticleManager.Instance.PlayEffect(trapData.deadParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
                    1f);
            }
            Destroy(gameObject);
        }


        #region ����and���սӿڷ���ʵ��

        public virtual void InteractPositive(GameObject interactor)
        {
            if (trapData == null) return;
            //��Ҫ��ý
            if (trapData.triggerType == TrapTriggerType.Negative) return;
            //�ж������Ƿ���Ч
            if (trapState != TrapState.Ready) return;
            //������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.interactSoundName))
            {
                AudioManager.Instance.PlaySfx(trapData.interactSoundName);
            }

            if (!string.IsNullOrEmpty(trapData.interactParticleName))
            {
                ParticleManager.Instance.PlayEffect(trapData.interactParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
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
            //������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.interactSoundName))
            {
                AudioManager.Instance.PlaySfx(trapData.interactSoundName);
            }

            if (!string.IsNullOrEmpty(trapData.interactParticleName))
            {
                ParticleManager.Instance.PlayEffect(trapData.interactParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
                    1f);
            }

            RemoveFormPlayerUsingList();
            this.interactor = interactor;
            ChangeState(TrapState.Running);
        }

        public override void Pick()
        {
            RemoveFormPlayerUsingList();
            MsgCenter.SendMsg(MsgConst.ON_PICK_ITEM, trapData.id, UseItemType.trap);
            //������Ч����Ч
            if (!string.IsNullOrEmpty(trapData.pickSoundName))
            {
                AudioManager.Instance.PlaySfx(trapData.pickSoundName);
            }

            if (!string.IsNullOrEmpty(trapData.pickParticleName))
            {
                ParticleManager.Instance.PlayEffect(trapData.pickParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
                    1f);
            }

            UIHelper.Instance.ShowOneTip(new TipInfo("�����" + EntityName + "��1", transform.position));
            ChangeState(TrapState.Dead);
        }

        #endregion

        #region ������

        /// <summary>
        /// ���崥����Ч������
        /// </summary>
        public virtual void Trigger()
        {
            if (!string.IsNullOrEmpty(trapData.workSoundName))
            {
                AudioManager.Instance.PlaySfx(trapData.workSoundName);
            }

            if (!string.IsNullOrEmpty(trapData.workParticleName))
            {
                ParticleManager.Instance.PlayEffect(trapData.workParticleName,
                    transform.position,
                    Quaternion.identity,
                    null,
                    true,
                    1f);
            }
            if(Rb) Rb.isKinematic = false;
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

        /// <summary>
        /// ����һ����ͻ����б����Ƴ�&&��������Ϣ�б����Ƴ�
        /// </summary>
        private void RemoveFormPlayerUsingList()
        {
            coll.enabled = false;//���̹رմ�����ײ ��ֹ�������̫��
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
        }


        /// <summary>
        /// �������չʾ�������飨��Χ��
        /// </summary>
        public void ShowDetail()
        {
            if (!hasInited) return;
            DetailGO.SetActive(true);
        }
        public void HideDetail()
        {
            if (!hasInited) return;
            DetailGO.SetActive(false);
        }

        /// <summary>
        /// չʾ�������Ԥ��
        /// </summary>
        public void ShowPlacePreview()
        {
            model.SetActive(false);
            //rb.isKinematic = true;
            coll.enabled = false;
            //naveObstacle.enabled = false;
            ReadyIndicator.gameObject.SetActive(false);
            previewGO.SetActive(true);
        }

        /// <summary>
        /// �����Ƿ���Է��õ�״̬
        /// </summary>
        /// <param name="canPlace"></param>
        public void SetCanPlaceState(bool canPlace)
        {
            canPlaceTrap = canPlace;
            if(canPlaceTrap)
            {
                previewGO.GetComponent<Renderer>().material.SetColor("_MainColor", CanPlaceColor);
                previewGO.GetComponent<Renderer>().material.SetColor("_OccludedColor", CanPlaceColor);
            }
            else
            {
                previewGO.GetComponent<Renderer>().material.SetColor("_MainColor", NoCanPlaceColor);
                previewGO.GetComponent<Renderer>().material.SetColor("_OccludedColor", NoCanPlaceColor);
            }
        }
    }
}