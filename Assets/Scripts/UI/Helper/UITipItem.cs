using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UITipItem : MonoBehaviour
    {
        public Text ContentText;

        private GameObject creator;
        private RectTransform rectTran;
        private Vector2 screenPos;
        private float originalY;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(GameObject creator, string content)
        {
            ContentText.text = content;
            this.creator = creator;

            // 初始状态
            transform.localScale = Vector3.zero;

            // 计算位置
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // 播放动画
            PlayShowAnimation();
        }

        public void InitWithRectTransform(GameObject creator, string content)
        {
            ContentText.text = content;
            this.creator = creator;

            // 初始状态
            transform.localScale = Vector3.one;

            // 计算位置
            screenPos = this.creator.GetComponent<RectTransform>().anchoredPosition;
            rectTran.localPosition = screenPos;
            originalY = rectTran.localPosition.y;

            // 播放动画
            PlayShowAnimation();
        }

        private void PlayShowAnimation()
        {
            Sequence seq = DOTween.Sequence();
            // 缩放动画
            seq.Append(transform.DOScale(Vector3.one, 0.3f));
            // 上移动画
            seq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        // 更新位置
        //private void Update()
        //{
        //    if (creator != null)
        //    {
        //        screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
        //        Vector2 newPos = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
        //        rectTran.localPosition = new Vector2(newPos.x, originalY + 50f); // 保持Y轴偏移
        //    }
        //}
    }
}