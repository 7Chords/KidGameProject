using DG.Tweening;
using KidGame.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    /// <summary>
    /// 通用的进度条UIItem
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

        private string circleProgressKey;
        private CircleProgressType progressType;
        private Action onComplete;

        private Sequence seq;
        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }

        public void Init(string circleProgressKey,CircleProgressType progressType,GameObject creator, float duration,Action onComplete)
        {
            MsgCenter.RegisterMsg(MsgConst.ON_MANUAL_CIRCLE_PROGRESS_CHG, onManualCircleProgressChg);

            this.circleProgressKey = circleProgressKey;
            this.creator = creator;
            maxProgress = duration;
            curProgress = 0;
            this.progressType = progressType;
            this.onComplete = onComplete;
            // 初始状态
            transform.localScale = Vector3.one;


            // 计算位置
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);

            // 放大
            seq = DOTween.Sequence().Append(transform.DOScale(Vector3.one, 0.3f));

            ProgressBarImage.fillAmount = 0;
        }

        private void OnDestroy()
        {
            MsgCenter.UnregisterMsg(MsgConst.ON_MANUAL_CIRCLE_PROGRESS_CHG, onManualCircleProgressChg);
            seq.Kill();
        }
        // 更新位置
        private void Update()
        {
            if(progressType == CircleProgressType.Auto)
            {
                curProgress += Time.deltaTime;
                ProgressBarImage.fillAmount = curProgress / maxProgress;
                if(curProgress >= maxProgress)
                {
                    onComplete?.Invoke();
                    Destroy(gameObject);
                }
            }
            if (creator != null)
            {
                screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
                Vector2 newPos = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
                rectTran.localPosition = new Vector2(newPos.x + OffsetX, newPos.y + OffsetY);
            }
        }

        private void onManualCircleProgressChg(object[] objs)
        {
            if (objs == null || objs.Length == 0) return;
            string key = objs[0] as string;
            if (key != circleProgressKey) return;
            float progressRate = (float)objs[1];
            ProgressBarImage.fillAmount = progressRate;
        }
    }
}