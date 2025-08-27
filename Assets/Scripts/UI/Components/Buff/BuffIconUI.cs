using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace KidGame.Core
{
    public class BuffIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        public Image buffIconImage;
        public Image cooldownOverlay;
        public Text stackCountText;
        
        private BuffInfo currentBuffInfo;
        private Coroutine tooltipCoroutine;
        
        public void Initialize(BuffInfo buffInfo, bool showStack, bool showDur)
        {
            currentBuffInfo = buffInfo;
            
            if (buffIconImage != null && buffInfo.buffData.icon != null)
            {
                buffIconImage.sprite = buffInfo.buffData.icon;
            }
            
            // 设置堆叠数
            if (stackCountText != null)
            {
                stackCountText.gameObject.SetActive(showStack && buffInfo.buffData.maxStack > 1);
                if (showStack && buffInfo.buffData.maxStack > 1)
                {
                    stackCountText.text = buffInfo.curStack.ToString();
                }
            }

            UpdateInfo(buffInfo, showDur);
        }
        
        public void UpdateInfo(BuffInfo buffInfo, bool showDur)
        {
            currentBuffInfo = buffInfo;
            
            // 更新堆叠数
            if (stackCountText != null && stackCountText.gameObject.activeSelf)
            {
                stackCountText.text = buffInfo.curStack.ToString();
            }
            
            // 更新冷却覆盖层
            if (cooldownOverlay != null && !buffInfo.buffData.isForever)
            {
                float fillAmount = Mathf.Clamp01(buffInfo.durationTimer / buffInfo.buffData.duration);
                cooldownOverlay.fillAmount = 1 - fillAmount;
            }
        }

        #region tooltip

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            if (tooltipCoroutine != null)
            {
                StopCoroutine(tooltipCoroutine);
            }
            tooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(0.5f));
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit");
            if (tooltipCoroutine != null)
            {
                StopCoroutine(tooltipCoroutine);
            }
            HideTooltip();
        }
        
        private IEnumerator ShowTooltipAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ShowTooltip();
        }
        
        private void ShowTooltip()
        {
            if (currentBuffInfo != null && currentBuffInfo.buffData != null)
            {
                var tempCell = new GameObject("TempBuffCell").AddComponent<CellUI>();
                tempCell.detailPoint = this.transform as RectTransform;
                tempCell.detailText = $"<b>{currentBuffInfo.buffData.buffName}</b>\n{currentBuffInfo.buffData.description}";
                
                UIHelper.Instance.ShowItemDetail(tempCell);
            }
        }
        
        private void HideTooltip()
        {
            UIHelper.Instance.HideItemDetail();
        }
        
        private void OnDestroy()
        {
            HideTooltip();
            if (tooltipCoroutine != null)
            {
                StopCoroutine(tooltipCoroutine);
            }
        }

        #endregion
    }
}