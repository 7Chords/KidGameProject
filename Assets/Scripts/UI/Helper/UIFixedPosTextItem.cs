using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UIFixedPosTextItem : MonoBehaviour
    {
        public Text ContentText;

        private RectTransform rectTran;
        private float originalY;
        private Sequence seq;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(FixedUIPosType posType,string content)
        {
            ContentText.text = content;

            // ��ʼ״̬
            transform.localScale = Vector3.zero;

            switch (posType)
            {
                case FixedUIPosType.Left:
                    rectTran.localPosition = UIHelper.Instance.LeftShowPos.localPosition;
                    break;
                case FixedUIPosType.Right:
                    rectTran.localPosition = UIHelper.Instance.RightShowPos.localPosition;
                    break;
                case FixedUIPosType.Top:
                    rectTran.localPosition = UIHelper.Instance.TopShowPos.localPosition;
                    break;
                case FixedUIPosType.Bottom:
                    rectTran.localPosition = UIHelper.Instance.BottomShowPos.localPosition;
                    break;
                case FixedUIPosType.Center:
                    rectTran.localPosition = UIHelper.Instance.CenterShowPos.localPosition;
                    break;
                default:
                    break;
            }

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