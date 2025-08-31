using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UIBubbleItem : MonoBehaviour
    {
        public Text ContentText;
        public Image ContentImage;
        public Text KeyText;
        public Text ItemNameText;

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
            ItemNameText.text = info.itemName;
            KeyText.text = key;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);
        }

        public void UpdateContent(string content)
        {
            ContentText.text = content;
        }

        private void FixedUpdate()
        {
            if (creator)
            {
                SetPosition(creator);
            }
        }

        /// <summary>
        /// 实时刷新气泡位置
        /// </summary>
        /// <param name="creator"></param>
        public void SetPosition(GameObject creator)
        {
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            // 屏幕坐标转UI坐标
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
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

        private void OnDestroy()
        {
            _tweener.Kill();
        }
    }
}