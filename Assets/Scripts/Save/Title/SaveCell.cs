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

            // ���浵�Ƿ����
            hasSaveData = slotIndex >= 0 && 
                         slotIndex < RecordData.recordNum && 
                         !string.IsNullOrEmpty(RecordData.Instance.recordName[slotIndex]);

            if (recordName != null)
            {
                if (!hasSaveData)
                {
                    recordName.text = "�մ浵";
                    recordName.color = Color.gray;
                    if (additionalInfo != null) 
                    {
                        additionalInfo.text = "�����������Ϸ";
                        additionalInfo.color = Color.gray;
                    }
                }
                else
                {
                    try
                    {
                        string fullName = RecordData.Instance.recordName[slotIndex];
                        
                        // ��ʾ�浵����
                        recordName.text = fullName;
                        recordName.color = Color.white;

                        // ���Զ�ȡ�浵���ݻ�ȡ������Ϣ
                        if (additionalInfo != null)
                        {
                            var saveData = PlayerSaveData.Instance.ReadForShow(slotIndex);
                            if (saveData != null)
                            {
                                string timeInfo = FormatPlayTime(saveData.totalPlayTimeSeconds);
                                additionalInfo.text = $"�ؿ�:{saveData.level} ����:{saveData.currentDay}\nʱ��:{timeInfo}";
                                additionalInfo.color = Color.white;
                            }
                            else
                            {
                                additionalInfo.text = "�浵���ݶ�ȡ��...";
                                additionalInfo.color = Color.yellow;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        recordName.text = "�浵��";
                        recordName.color = Color.red;
                        if (additionalInfo != null)
                        {
                            additionalInfo.text = "��������޸�";
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
                // �������Ч��
                StartCoroutine(ClickAnimation());
                
                // ��������¼�
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