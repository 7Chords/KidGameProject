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
        public Button deleteButton;

        public Color highlightColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        public Color normalColor = Color.white;
        public Color pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        public static System.Action<int> OnLeftClick;
        public static System.Action<int> OnEnter;
        public static System.Action OnExit;
        public static System.Action<int> OnDeleteClick;

        private int slotIndex = -1;
        private bool hasSaveData = false;

        public void Initialize(int index)
        {
            slotIndex = index;
            // 绑定删除按钮
            if (deleteButton != null)
            {
                deleteButton.onClick.RemoveAllListeners();
                deleteButton.onClick.AddListener(() => { OnDeleteClick?.Invoke(slotIndex); });
            }

            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            if (indexText != null) indexText.text = (slotIndex + 1).ToString();

            // 先看是否有名字
            bool named = slotIndex >= 0 && slotIndex < RecordData.recordNum &&
                         !string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]);

            var preview = named ? PlayerSaveData.Instance.ReadForShow(slotIndex) : null;
            hasSaveData = preview != null;

            if (!hasSaveData)
            {
                recordName.text = "空存档";
                recordName.color = Color.gray;
                if (additionalInfo != null)
                {
                    additionalInfo.text = "点击创建新游戏";
                    additionalInfo.color = Color.gray;
                }
                background.color = normalColor;
                return;
            }

            // 有效存档
            recordName.text = RecordData.Instance.recordName[slotIndex];
            recordName.color = Color.white;
            if (additionalInfo != null)
            {
                string timeInfo = FormatPlayTime(preview.totalPlayTimeSeconds);
                additionalInfo.text = $"关卡:{preview.level} 天数:{preview.currentDay}\n时间:{timeInfo}";
                additionalInfo.color = Color.white;
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
                StartCoroutine(ClickAnimation());
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