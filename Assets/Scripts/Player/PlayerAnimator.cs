using UnityEngine;

namespace KidGame.Core
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void PlayAnimation(string animationName)
        {
            animator.Play(animationName);
        }

        #region AnimationEvent

        /// <summary>
        /// ³å´Ì½áÊø
        /// </summary>
        public void OnDashOver()
        {
            PlayerController.Instance.ChangeState(PlayerState.Idle);
        }

        #endregion
    }
}
