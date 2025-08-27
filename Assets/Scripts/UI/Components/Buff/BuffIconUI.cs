using System;
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
        
        [Header("测试Buff数据")]
        public Sprite testIcon;
        public string buffName = "测试Buff";
        public string buffDescription = "这是一个测试Buff的描述信息，用于验证Tooltip功能是否正常工作";
        
        private BuffInfo currentBuffInfo;
        private BuffData testBuffData;
        private bool isShowingTooltip = false;

        private void Start()
        {
            testBuffData = ScriptableObject.CreateInstance<BuffData>();
            testBuffData.id = "test_buff";
            testBuffData.buffName = buffName;
            testBuffData.description = buffDescription;
            testBuffData.icon = testIcon;
            testBuffData.maxStack = 3;
            testBuffData.isForever = true;
        }

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
            if (!isShowingTooltip)
            {
                ShowTooltip();
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }
        
        private void ShowTooltip()
        {
            if (UIHelper.Instance == null) return;
            
            isShowingTooltip = true;
            
            if (currentBuffInfo != null && currentBuffInfo.buffData != null)
            {
                UIHelper.Instance.ShowBuffDetail(
                    this.transform,
                    currentBuffInfo.buffData.buffName,
                    currentBuffInfo.buffData.description
                );
            }
            else
            {
                // 使用测试数据
                UIHelper.Instance.ShowBuffDetail(
                    this.transform,
                    buffName,
                    buffDescription
                );
            }
        }
        
        private void HideTooltip()
        {
            if (UIHelper.Instance != null && isShowingTooltip)
            {
                UIHelper.Instance.HideBuffDetail();
                isShowingTooltip = false;
            }
        }
        
        private void OnDestroy()
        {
            HideTooltip();
        }

        #endregion
    }
}