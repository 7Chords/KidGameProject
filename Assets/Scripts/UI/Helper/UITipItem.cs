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

            // ��ʼ״̬
            transform.localScale = Vector3.zero;

            // ����λ��
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // ���Ŷ���
            PlayShowAnimation();
        }

        public void InitWithRectTransform(GameObject creator, string content)
        {
            ContentText.text = content;
            this.creator = creator;

            // ��ʼ״̬
            transform.localScale = Vector3.one;

            // ����λ��
            screenPos = this.creator.GetComponent<RectTransform>().anchoredPosition;
            rectTran.localPosition = screenPos;
            originalY = rectTran.localPosition.y;

            // ���Ŷ���
            PlayShowAnimation();
        }

        private void PlayShowAnimation()
        {
            Sequence seq = DOTween.Sequence();
            // ���Ŷ���
            seq.Append(transform.DOScale(Vector3.one, 0.3f));
            // ���ƶ���
            seq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        // ����λ��
        //private void Update()
        //{
        //    if (creator != null)
        //    {
        //        screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
        //        Vector2 newPos = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
        //        rectTran.localPosition = new Vector2(newPos.x, originalY + 50f); // ����Y��ƫ��
        //    }
        //}
    }
}