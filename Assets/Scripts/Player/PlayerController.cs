using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : Singleton<PlayerController>, IStateMachineOwner,IDamageable
    {
        private InputSettings inputSettings;
        public InputSettings InputSettings => inputSettings;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        public PlayerAnimator PlayerAnimator;

        public PlayerBaseData PlayerBaseData;

        public Transform ModelTransform;

        private StateMachine stateMachine;
        private PlayerState playerState; // 玩家的当前状态

        private BuffHandler playerBuffHandler;

        //key:可交互 value:和玩家距离
        private Dictionary<IInteractive,float> interactiveDict;

        //key:可回收 value:和玩家距离
        private Dictionary<IRecyclable, float> recyclableDict;

        protected override void Awake()
        {
            base.Awake();
            inputSettings = GetComponent<InputSettings>();

            rb = GetComponent<Rigidbody>();
        }

        public void Init()
        {
            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //初始化为Idle状态
            ChangeState(PlayerState.Idle);
            //初始化buff处理器
            playerBuffHandler = new BuffHandler();
            playerBuffHandler.Init();

            //注册一些事件
            RegActions();

            //初始化字典
            interactiveDict = new Dictionary<IInteractive, float>();
            recyclableDict = new Dictionary<IRecyclable, float>();
        }

        public void Discard()
        {
            stateMachine.ObjectPushPool();
            playerBuffHandler.Discard();
            UnregActions();
        }

        /// <summary>
        /// 玩家旋转 TODO:优化？
        /// </summary>
        public void Rotate(Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }


        /// <summary>
        /// 修改状态
        /// </summary>
        public void ChangeState(PlayerState playerState, bool reCurrstate = false)
        {
            this.playerState = playerState;
            switch (playerState)
            {
                case PlayerState.Idle:
                    stateMachine.ChangeState<PlayerIdleState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Move:
                    stateMachine.ChangeState<PlayerMoveState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Dash:
                    stateMachine.ChangeState<PlayerDashState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Use:
                    stateMachine.ChangeState<PlayerUseState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Throw:
                    stateMachine.ChangeState<PlayerThrowState>((int)playerState, reCurrstate);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            PlayerAnimator.PlayAnimation(animationName);
        }

        #region 事件相关

        /// <summary>
        /// 注册事件们
        /// </summary>
        private void RegActions()
        {
            inputSettings.OnInteractionPress += PlayerInteraction;
            inputSettings.OnRecyclePress += PlayerRecycle;
            inputSettings.OnUsePress += TryPlaceTrap;
        }

        /// <summary>
        /// 反注册事件们
        /// </summary>
        private void UnregActions()
        {
            inputSettings.OnInteractionPress -= PlayerInteraction;
            inputSettings.OnRecyclePress -= PlayerRecycle;
            inputSettings.OnUsePress -= TryPlaceTrap;
        }

        #endregion


        #region 功能
        /// <summary>
        /// 玩家交互
        /// </summary>
        public void PlayerInteraction()
        {
            if (interactiveDict == null || interactiveDict.Count == 0) return;
            GetClosestInteractive()?.InteractPositive();
        }

        public void PlayerRecycle()
        {
            if (recyclableDict == null || recyclableDict.Count == 0) return;
            GetClosestRecyclable()?.Recycle();
        }

        /// <summary>
        /// 尝试放陷阱
        /// </summary>
        public void TryPlaceTrap()
        {
            if (PlayerBag.Instance._trapBag.Count > 0)
            {
                ChangeState(PlayerState.Use);
            }
        }

        public void TryThrowTrap()
        {
            if (PlayerBag.Instance._trapBag.Count > 0)
            {
                ChangeState(PlayerState.Throw);
            }
        }

        /// <summary>
        /// 添加到可交互列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddInteractiveToList(IInteractive interactive,float distance)
        {
            if (interactiveDict == null) return;
            if (interactiveDict.ContainsKey(interactive)) return;
            interactiveDict.Add(interactive,distance);
        }

        /// <summary>
        /// 从可交互列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveInteractiveFromList(IInteractive interactive)
        {
            if (interactiveDict == null) return;
            if (!interactiveDict.ContainsKey(interactive)) return;
            interactiveDict.Remove(interactive);
        }

        /// <summary>
        /// 获得最近的可交互者
        /// </summary>
        private IInteractive GetClosestInteractive()
        {
            float min = 999;
            IInteractive closestInteractive = null;
            foreach(var pair in interactiveDict)
            {
                if(pair.Value<min)
                {
                    min = pair.Value;
                    closestInteractive = pair.Key;
                }
            }
            return closestInteractive;
        }

        /// <summary>
        /// 添加到可回收列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddIRecyclableToList(IRecyclable recyclable, float distance)
        {
            if (recyclableDict == null) return;
            if (recyclableDict.ContainsKey(recyclable)) return;
            recyclableDict.Add(recyclable, distance);
        }

        /// <summary>
        /// 从可回收列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveRecyclableFromList(IRecyclable recyclable)
        {
            if (recyclableDict == null) return;
            if (!recyclableDict.ContainsKey(recyclable)) return;
            recyclableDict.Remove(recyclable);
        }

        /// <summary>
        /// 获得最近的可回收者
        /// </summary>
        private IRecyclable GetClosestRecyclable()
        {
            float min = 999;
            IRecyclable closestIRecyclable = null;
            foreach (var pair in recyclableDict)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    closestIRecyclable = pair.Key;
                }
            }
            return closestIRecyclable;
        }



        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="damageInfo"></param>
        public void TakeDamage(DamageInfo damageInfo)
        {
            
        }

        #endregion


        #region Gizoms

        private void OnDrawGizmos()
        {
        }

        #endregion
    }
}