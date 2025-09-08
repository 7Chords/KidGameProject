using KidGame.Interface;
using KidGame.UI;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// 陷阱基类
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
        /// 初始化
        /// </summary>
        /// <param name="trapData"></param>
        public virtual void Init(TrapData trapData)
        {
            this.trapData = trapData;

            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //初始化为Idle状态
            ChangeState(TrapState.NoReady);

            //开启交互触发器
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


        #region 交互and回收接口方法实现

        public virtual void InteractPositive(GameObject interactor)
        {
            if (trapData == null) return;
            //需要触媒
            if (trapData.triggerType == TrapTriggerType.Negative) return;
            //判断陷阱是否有效
            if (trapState != TrapState.Ready) return;
            //播放音效和特效
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
            //判断陷阱是否有效
            if (trapState != TrapState.Ready) return;
            //播放音效和特效
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
            //播放音效和特效
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

            UIHelper.Instance.ShowOneTip(new TipInfo("获得了" + EntityName + "×1", transform.position));
            ChangeState(TrapState.Dead);
        }

        #endregion

        #region 功能性

        /// <summary>
        /// 陷阱触发的效果代码
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
        /// 外部事件导致陷阱死亡
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
        /// 修改状态
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
        /// 从玩家互动和回收列表中移除&&从气泡信息列表中移除
        /// </summary>
        private void RemoveFormPlayerUsingList()
        {
            coll.enabled = false;//立刻关闭触发碰撞 防止玩家手速太快
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
        }


        /// <summary>
        /// 鼠标移入展示陷阱详情（范围）
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
        /// 展示陷阱放置预览
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
        /// 设置是否可以放置的状态
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