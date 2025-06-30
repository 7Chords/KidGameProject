using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UITipItem : MonoBehaviour
    {
        public Text ContentText;

        private GameObject creator;
        private GameObject player;

        //private Tweener _tweener;

        private RectTransform rectTran;

        private Vector2 screenPos;
        private float originalY;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(GameObject creator,string content)
        {
            ContentText.text = content;
            this.creator = creator;
            transform.localScale = Vector3.zero;
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            // ÆÁÄ»×ø±ê×ªUI×ø±ê
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
            originalY = rectTran.localPosition.y;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one, 0.3f));
            seq.Append(rectTran.DOLocalMoveY(originalY + 50f, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject);
            }));
        }
    }
}
