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

            // ��ʼ״̬
            transform.localScale = Vector3.zero;

            rectTran.localPosition = UIHelper.Instance.SideShowPos.localPosition;
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