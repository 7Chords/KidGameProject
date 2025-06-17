using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : Singleton<PlayerController>, IStateMachineOwner
    {
        private InputSettings inputSettings;
        public InputSettings InputSettings => inputSettings;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Animator animator;

        private StateMachine stateMachine;
        private PlayerState playerState; // 玩家的当前状态

        public PlayerBaseData PlayerBaseData;

        public Transform ModelTransform;

        //key:可交互 value:和玩家距离
        private Dictionary<IInteractive,float> interactiveDict;

        protected override void Awake()
        {
            base.Awake();
            inputSettings = GetComponent<InputSettings>();

            animator = transform.GetChild(0).GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }

        public void Init()
        {
            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //初始化为Idle状态
            ChangeState(PlayerState.Idle);
            //注册一些事件
            RegActions();

            interactiveDict = new Dictionary<IInteractive, float>();
        }

        public void Discard()
        {
            stateMachine.ObjectPushPool();
            UnregActions();
        }

        /// <summary>
        /// 玩家旋转 TODO:优化？
        /// </summary>
        public void Rotate()
        {
            // 将鼠标屏幕坐标转换为世界坐标
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.transform.position.y - transform.position.y; // 设置Z轴为相机与玩家的高度差
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 计算从玩家位置指向鼠标位置的方向向量
            Vector3 direction = worldMousePosition - transform.position;
            direction.y = 0; // 确保只在水平面上旋转

            // 如果方向向量有效，则旋转玩家
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
            }
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
        /// 播放动画 TODO:后续封装一个玩家动画控制器？
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayerAnimation(string animationName)
        {
            animator.Play(animationName);
        }

        #region 事件相关

        /// <summary>
        /// 注册事件们
        /// </summary>
        private void RegActions()
        {
            inputSettings.OnInteractionPress += PlayerInteraction;
            inputSettings.OnUsePress += TryPlaceTrap;
        }

        /// <summary>
        /// 反注册事件们
        /// </summary>
        private void UnregActions()
        {
            inputSettings.OnInteractionPress -= PlayerInteraction;
            inputSettings.OnUsePress -= TryPlaceTrap;
        }

        /// <summary>
        /// 玩家交互
        /// </summary>
        public void PlayerInteraction()
        {
            if (interactiveDict == null || interactiveDict.Count == 0) return;
            GetClosestInteractive()?.InteractPositive();
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

        /// <summary>
        /// 添加到可交互列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddInteractiveToList(IInteractive interactive,float distance)
        {
            if (interactiveDict == null) return;
            interactiveDict.Add(interactive,distance);
        }

        /// <summary>
        /// 从可交互列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveInteractiveFromList(IInteractive interactive)
        {
            if (interactiveDict == null) return;
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

        #endregion

        #region Gizoms

        private void OnDrawGizmos()
        {
        }

        #endregion
    }
}