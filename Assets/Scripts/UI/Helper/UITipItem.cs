using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UITipItem : MonoBehaviour
    {
        public Text ContentText;

        private Vector3 creatorPos;
        private RectTransform rectTran;
        private Vector2 screenPos;
        private float originalY;
        private Sequence animSeq;
        private Sequence deadSequence;
        private float deadTime;
        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(Vector3 creatorPos, string content,float deadTime)
        {
            ContentText.text = content;
            this.creatorPos = creatorPos;
            this.deadTime = deadTime;
            // 初始状态
            transform.localScale = Vector3.zero;

            // 计算位置
            screenPos = Camera.main.WorldToScreenPoint(creatorPos) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // 播放动画
            PlayShowAnimation();
            deadSequence = DOTween.Sequence().AppendInterval(deadTime).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        //public void InitWithRectTransform(GameObject creator, string content)
        //{
        //    ContentText.text = content;
        //    this.creator = creator;

        //    // 初始状态
        //    transform.localScale = Vector3.one;

        //    // 计算位置
        //    screenPos = this.creator.GetComponent<RectTransform>().anchoredPosition;
        //    rectTran.localPosition = screenPos;
        //    originalY = rectTran.localPosition.y;

        //    // 播放动画
        //    PlayShowAnimation();
        //}
        private void PlayShowAnimation()
        {
            animSeq = DOTween.Sequence();
            // 缩放动画
            animSeq.Append(transform.DOScale(Vector3.one, 0.3f));
            // 上移动画
            animSeq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        public void ResetTip(Vector3 creatorPos, string content)
        {
            animSeq.Kill();
            deadSequence.Kill();
            ContentText.text = content;
            this.creatorPos = creatorPos;
            // 初始状态
            transform.localScale = Vector3.zero;
            // 计算位置
            screenPos = Camera.main.WorldToScreenPoint(creatorPos) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // 播放动画
            PlayShowAnimation();
            deadSequence = DOTween.Sequence().AppendInterval(deadTime).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        private void OnDestroy()
        {
            animSeq.Kill();//不销毁的话如果物体被销毁序列依然存在 产生waring
            deadSequence.Kill();
        }
    }
}