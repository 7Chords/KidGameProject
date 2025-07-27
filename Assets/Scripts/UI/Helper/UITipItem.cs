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
        private Sequence seq;
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
            seq = DOTween.Sequence();
            // ���Ŷ���
            seq.Append(transform.DOScale(Vector3.one, 0.3f));
            // ���ƶ���
            seq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f));
        }

        private void OnDestroy()
        {
            seq.Kill();//�����ٵĻ�������屻����������Ȼ���� ����waring
        }
    }
}