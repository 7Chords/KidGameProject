using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// 玩家旋转
        /// </summary>
        public void Rotate()
        {
            //将鼠标屏幕坐标转换为世界坐标
            playerInfo.MouseWorldPos = MouseRaycaster.Instance.GetMousePosi();
            //忽略Y轴差异
            playerInfo.PlayerBottomPos = new Vector3(transform.position.x, playerInfo.MouseWorldPos.y, transform.position.z);
            //计算方向向量
            playerInfo.RotateDir = (playerInfo.MouseWorldPos - playerInfo.PlayerBottomPos).normalized;
            transform.rotation = Quaternion.LookRotation(playerInfo.RotateDir);
        }

        public void ChangeState(PlayerState playerState, bool reCurrstate = false)
        {
            // 如果处于体力耗尽状态只能进入Idle状态
            if (playerInfo.IsExhausted && (playerState == PlayerState.Move || playerState == PlayerState.Dash))
            {
                playerState = PlayerState.Idle;
            }

            playerInfo.PlayerState = playerState;
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
                case PlayerState.Dead:
                    stateMachine.ChangeState<PlayerDeadState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Struggle:
                    stateMachine.ChangeState<PlayerStruggleState>((int)playerState, reCurrstate);
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
    }
}