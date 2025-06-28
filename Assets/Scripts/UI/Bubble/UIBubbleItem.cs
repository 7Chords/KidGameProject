using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UIBubbleItem : MonoBehaviour
    {
        public Text ContentText; //��ʾ�����ݣ��磺��ʹ��/Ͷ��/������
        public Image ContentImage; //��ʾ��ͼƬ����ʱû�ã�
        public Text KeyText; //��ʾ�ļ�λ

        private GameObject creator;
        private GameObject player;

        private Tweener _tweener;

        private RectTransform rectTran;

        Vector2 screenPos;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }
        public void Init(BubbleInfo info, string key)
        {
            creator = info.creator;
            player = info.player;
            ContentText.text = info.content;
            KeyText.text = key;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);
        }

        private void FixedUpdate()
        {
            if (creator)
            {
                SetPosition(creator);
            }
        }

        /// <summary>
        /// ����Һ���Ҫ��������Ʒ��λ�ü����ø�����
        /// </summary>
        /// <param name="creator"></param>
        public void SetPosition(GameObject creator)
        {
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            // ת��Ϊ UI ��������
            rectTran.localPosition = ScreenPointToUIPoint(rectTran, screenPos);
        }

        public void DestoryBubble()
        {
            _tweener = transform.DOScale(0, 0.2f);
            StartCoroutine(WaitToDestory());
        }

        private IEnumerator WaitToDestory()
        {
            yield return _tweener.WaitForCompletion();
            Destroy(gameObject);
        }

        // ��Ļ����ת��Ϊ UGUI ����
        public Vector2 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt.parent as RectTransform,
                screenPoint,
                null,
                out localPoint
            );
            return localPoint;
        }
    }
}