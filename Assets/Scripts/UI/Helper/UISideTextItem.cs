using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UISideTextItem : MonoBehaviour
    {
        public Text ContentText;

        private RectTransform rectTran;
        private float originalY;
        private Sequence seq;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(string content)
        {
            ContentText.text = content;

            // 初始状态
            transform.localScale = Vector3.zero;

            rectTran.localPosition = UIHelper.Instance.SideShowPos.localPosition;
            originalY = rectTran.localPosition.y;

            // 播放动画
            PlayShowAnimation();
        }

        private void PlayShowAnimation()
        {
            seq = DOTween.Sequence();
            // 缩放动画
            seq.Append(transform.DOScale(Vector3.one, 0.3f));
            // 上移动画
            seq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        private void OnDestroy()
        {
            seq.Kill();//不销毁的话如果物体被销毁序列依然存在 产生waring
        }
    }
}