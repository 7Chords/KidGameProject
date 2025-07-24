using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    /// <summary>
    /// ͨ�õĽ�����UIItem
    /// </summary>
    public class UICommonProgressItem : MonoBehaviour
    {
        public Image ProgressBarImage;

        private RectTransform rectTran;
        private GameObject creator;
        private Vector2 screenPos;

        public float OffsetX = 30f;
        public float OffsetY = 30f;

        private float curProgress;
        private float maxProgress;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(GameObject creator, float duration)
        {
            this.creator = creator;
            maxProgress = duration;
            curProgress = 0;
            // ��ʼ״̬
            transform.localScale = Vector3.one;


            // ����λ��
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);

            // �Ŵ�
            DOTween.Sequence().Append(transform.DOScale(Vector3.one, 0.3f));
        }


        // ����λ��
        private void Update()
        {
            curProgress += Time.deltaTime;
            ProgressBarImage.fillAmount = curProgress / maxProgress;
            if (creator != null)
            {
                screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
                Vector2 newPos = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
                rectTran.localPosition = new Vector2(newPos.x + OffsetX, newPos.y + OffsetY);
            }
        }
    }
}