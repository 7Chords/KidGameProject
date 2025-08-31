using KidGame.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace KidGame.UI
{
    public class SaveCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text indexText; // 序号文本
        public Text recordName; // 存档名称文本
        public Text additionalInfo; // 附加信息文本
        public Image background; // 背景图片

        public Color highlightColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        public Color normalColor = Color.white;
        public Color pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        public static System.Action<int> OnLeftClick;
        public static System.Action<int> OnEnter;
        public static System.Action OnExit;

        private int slotIndex = -1;
        private bool hasSaveData = false;

        public void Initialize(int index)
        {
            slotIndex = index;
            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            if (indexText != null)
            {
                indexText.text = (slotIndex + 1).ToString();
            }

            // 检查存档是否存在
            hasSaveData = slotIndex >= 0 && 
                         slotIndex < RecordData.recordNum && 
                         !string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]);

            if (recordName != null)
            {
                if (!hasSaveData)
                {
                    recordName.text = "空存档";
                    recordName.color = Color.gray;
                    if (additionalInfo != null) 
                    {
                        additionalInfo.text = "点击创建新游戏";
                        additionalInfo.color = Color.gray;
                    }
                }
                else
                {
                    try
                    {
                        string fullName = RecordData.Instance.recordName[slotIndex];
                        
                        // 显示存档名称
                        recordName.text = fullName;
                        recordName.color = Color.white;

                        // 尝试读取存档数据获取更多信息
                        if (additionalInfo != null)
                        {
                            var saveData = PlayerSaveData.Instance.ReadForShow(slotIndex);
                            if (saveData != null)
                            {
                                string timeInfo = FormatPlayTime(saveData.totalPlayTimeSeconds);
                                additionalInfo.text = $"关卡:{saveData.level} 天数:{saveData.currentDay}\n时间:{timeInfo}";
                                additionalInfo.color = Color.white;
                            }
                            else
                            {
                                additionalInfo.text = "存档数据读取中...";
                                additionalInfo.color = Color.yellow;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        recordName.text = "存档损坏";
                        recordName.color = Color.red;
                        if (additionalInfo != null)
                        {
                            additionalInfo.text = "点击尝试修复";
                            additionalInfo.color = Color.red;
                        }
                    }
                }
            }
        }

        private string FormatPlayTime(int totalSeconds)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(totalSeconds);
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 点击反馈效果
                StartCoroutine(ClickAnimation());
                
                // 触发点击事件
                OnLeftClick?.Invoke(slotIndex);
            }
        }

        private IEnumerator ClickAnimation()
        {
            if (background != null)
            {
                background.color = pressedColor;
                yield return new WaitForSeconds(0.1f);
                background.color = hasSaveData ? highlightColor : normalColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // 高亮背景
            if (background != null && hasSaveData)
            {
                background.color = highlightColor;
            }
            
            OnEnter?.Invoke(slotIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 恢复背景颜色
            if (background != null)
            {
                background.color = normalColor;
            }

            OnExit?.Invoke();
        }

        // 设置单元格是否可交互
        public void SetInteractable(bool interactable)
        {
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
            
            // 也可以调整透明度等来显示不可用状态
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = interactable ? 1f : 0.5f;
            }
        }
    }
}