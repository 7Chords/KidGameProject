using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UIBubbleItem : MonoBehaviour
    {
        public Text ContentText; //提示的内容，如：“使用/投掷/触发”
        public Image ContentImage; //提示的图片（暂时没用）
        public Text KeyText; //提示的键位

        private GameObject go_1;
        private GameObject go_2;

        private Tweener _tweener;

        private RectTransform rectTran;

        Vector3 worldCenterPos;
        Vector2 screenPos;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }
        public void Init(BubbleInfo info, string key)
        {
            go_1 = info.go_1;
            go_2 = info.go_2;
            ContentText.text = info.content;
            KeyText.text = key;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);

            //TODO:设置父物体
            transform.parent = transform;
        }

        private void Update()
        {
            if (go_1 && go_2)
            {
                SetPosition(go_1, go_2);
            }
        }

        /// <summary>
        /// 在玩家和需要交互的物品的位置间设置该气泡
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public void SetPosition(GameObject obj1, GameObject obj2)
        {
            worldCenterPos = (obj1.transform.position + obj2.transform.position) / 2;
            screenPos = Camera.main.WorldToScreenPoint(worldCenterPos);
            rectTran.transform.localPosition = ScreenPointToUIPoint(rectTran, screenPos);
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

        // 屏幕坐标转换为 UGUI 坐标
        public Vector3 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint)
        {
            Vector3 uiLocalPos;
            //TODO:应该有个ui相机才对
            Camera uiCamera = Camera.main;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out uiLocalPos);
            return uiLocalPos;
        }
    }
}
