using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// �����ת
        /// </summary>
        public void Rotate()
        {
            //�������Ļ����ת��Ϊ��������
            playerInfo.MouseWorldPos = MouseRaycaster.Instance.GetMousePosi();
            //����Y�����
            playerInfo.PlayerBottomPos = new Vector3(transform.position.x, playerInfo.MouseWorldPos.y, transform.position.z);
            //���㷽������
            playerInfo.RotateDir = (playerInfo.MouseWorldPos - playerInfo.PlayerBottomPos).normalized;
            transform.rotation = Quaternion.LookRotation(playerInfo.RotateDir);
        }

        public void ChangeState(PlayerState playerState, bool reCurrstate = false)
        {
            // ������������ľ�״ֻ̬�ܽ���Idle״̬
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
        /// ���Ŷ���
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            PlayerAnimator.PlayAnimation(animationName);
        }
    }
}