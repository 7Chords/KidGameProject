using KidGame.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace KidGame.UI
{
    public class SaveCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text indexText; // ����ı�
        public Text recordName; // �浵�����ı�
        public Text additionalInfo; // ������Ϣ�ı�
        public Image background; // ����ͼƬ
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
            // ��ɾ����ť
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

            // �ȿ��Ƿ�������
            bool named = slotIndex >= 0 && slotIndex < RecordData.recordNum &&
                         !string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]);

            var preview = named ? PlayerSaveData.Instance.ReadForShow(slotIndex) : null;
            hasSaveData = preview != null;

            if (!hasSaveData)
            {
                recordName.text = "�մ浵";
                recordName.color = Color.gray;
                if (additionalInfo != null)
                {
                    additionalInfo.text = "�����������Ϸ";
                    additionalInfo.color = Color.gray;
                }
                background.color = normalColor;
                return;
            }

            // ��Ч�浵
            recordName.text = RecordData.Instance.recordName[slotIndex];
            recordName.color = Color.white;
            if (additionalInfo != null)
            {
                string timeInfo = FormatPlayTime(preview.totalPlayTimeSeconds);
                additionalInfo.text = $"�ؿ�:{preview.level} ����:{preview.currentDay}\nʱ��:{timeInfo}";
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
            // ��������
            if (background != null && hasSaveData)
            {
                background.color = highlightColor;
            }
            
            OnEnter?.Invoke(slotIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // �ָ�������ɫ
            if (background != null)
            {
                background.color = normalColor;
            }

            OnExit?.Invoke();
        }

        // ���õ�Ԫ���Ƿ�ɽ���
        public void SetInteractable(bool interactable)
        {
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
            
            // Ҳ���Ե���͸���ȵ�����ʾ������״̬
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = interactable ? 1f : 0.5f;
            }
        }
    }
}