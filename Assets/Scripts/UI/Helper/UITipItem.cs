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
            // ��ʼ״̬
            transform.localScale = Vector3.zero;

            // ����λ��
            screenPos = Camera.main.WorldToScreenPoint(creatorPos) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // ���Ŷ���
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

        //    // ��ʼ״̬
        //    transform.localScale = Vector3.one;

        //    // ����λ��
        //    screenPos = this.creator.GetComponent<RectTransform>().anchoredPosition;
        //    rectTran.localPosition = screenPos;
        //    originalY = rectTran.localPosition.y;

        //    // ���Ŷ���
        //    PlayShowAnimation();
        //}
        private void PlayShowAnimation()
        {
            animSeq = DOTween.Sequence();
            // ���Ŷ���
            animSeq.Append(transform.DOScale(Vector3.one, 0.3f));
            // ���ƶ���
            animSeq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        public void ResetTip(Vector3 creatorPos, string content)
        {
            animSeq.Kill();
            deadSequence.Kill();
            ContentText.text = content;
            this.creatorPos = creatorPos;
            // ��ʼ״̬
            transform.localScale = Vector3.zero;
            // ����λ��
            screenPos = Camera.main.WorldToScreenPoint(creatorPos) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;

            // ���Ŷ���
            PlayShowAnimation();
            deadSequence = DOTween.Sequence().AppendInterval(deadTime).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        private void OnDestroy()
        {
            animSeq.Kill();//�����ٵĻ�������屻����������Ȼ���� ����waring
            deadSequence.Kill();
        }
    }
}