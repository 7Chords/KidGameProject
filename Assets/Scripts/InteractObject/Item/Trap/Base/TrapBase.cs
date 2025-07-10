using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 陷阱基类
    /// </summary>
    public class TrapBase : MapItem, IInteractive, IStateMachineOwner
    {
        [SerializeField] private List<string> randomInteractSfxList;

        public List<string> RandomInteractSfxList
        {
            get => randomInteractSfxList;
            set { randomInteractSfxList = value; }
        }

        [SerializeField] private GameObject interactPartical;

        public GameObject InteractPartical
        {
            get => interactPartical;
            set { interactPartical = value; }
        }

        public override string itemName => trapData.trapName;

        [ColorUsage(true, true)] public Color NoReadyColor;

        [ColorUsage(true, true)] public Color ReadyColor;

        public Renderer ReadyIndicator;

        [Space(20)] private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Collider coll;
        public Collider Coll => coll;


        protected TrapData trapData;
        public TrapData TrapData => trapData;


        protected CatalystBase _catalyst;
        public CatalystBase Catalyst => _catalyst;


        protected GameObject interactor;
        public GameObject Interactor => interactor;


        protected StateMachine stateMachine;
        private TrapState trapState;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
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
        }

        public virtual void Discard()
        {
            //todo:fix
            //stateMachine.Destory();
            Destroy(gameObject);
        }


        #region 交互接口方法实现

        public virtual void InteractPositive(GameObject interactor)
        {
            if (trapData == null) return;
            //需要触媒
            if (trapData.triggerType == TrapTriggerType.Negative) return;
            //判断陷阱是否有效
            if (trapState != TrapState.Ready) return;
            //todo:播放音效和特效
            if (RandomInteractSfxList != null && RandomInteractSfxList.Count > 0)
            {
                AudioManager.Instance.PlaySfx(RandomInteractSfxList[Random.Range(0, RandomInteractSfxList.Count)]);
            }

            if (interactPartical != null)
            {
                MonoManager.Instance.InstantiateGameObject(interactPartical, transform.position, Quaternion.identity,
                    1f);
            }

            RemoveFormPlayerUsingList();
            this.interactor = interactor;
            ChangeState(TrapState.Running);
        }


        public virtual void InteractNegative(CatalystBase catalyst, GameObject interactor)
        {
            if (trapData == null || trapData.triggerType != TrapTriggerType.Negative) return;
            if (_catalyst == null || _catalyst != catalyst) return;
            //判断陷阱是否有效
            if (trapState != TrapState.Ready) return;
            //todo:播放音效和特效
            if (RandomInteractSfxList != null && RandomInteractSfxList.Count > 0)
            {
                AudioManager.Instance.PlaySfx(RandomInteractSfxList[Random.Range(0, RandomInteractSfxList.Count)]);
            }

            if (interactPartical != null)
            {
                MonoManager.Instance.InstantiateGameObject(interactPartical, transform.position, Quaternion.identity,
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
            //todo:播放音效和特效
            if (RandomPickSfxList != null && RandomPickSfxList.Count > 0)
            {
                // AudioManager.Instance.PlaySfx(RandomPickSfxList[Random.Range(0, RandomPickSfxList.Count)]);
            }

            if (pickPartical != null)
            {
                MonoManager.Instance.InstantiateGameObject(pickPartical, transform.position, Quaternion.identity, 1f);
            }

            UIHelper.Instance.ShowTipImmediate(new TipInfo("获得了" + itemName + "×1", gameObject));
            ChangeState(TrapState.Dead);
        }

        #endregion

        #region 功能性

        /// <summary>
        /// 设置触媒
        /// </summary>
        /// <param name="catalyst"></param>
        public void SetCatalyst(CatalystBase catalyst)
        {
            _catalyst = catalyst;
            if (_catalyst == null)
            {
                ChangeState(TrapState.NoReady);
            }
        }

        /// <summary>
        /// 陷阱触发的效果代码
        /// </summary>
        public virtual void Trigger()
        {
            Debug.Log(gameObject.name + "陷阱触发了");
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


        private void RemoveFormPlayerUsingList()
        {
            coll.enabled = false;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
        }
    }
}