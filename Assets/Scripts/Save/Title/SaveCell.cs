using KidGame.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace KidGame.UI
{
    public class SaveCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text indexText; //���
        public Text recordName; //�浵��
        public Image rect; //����

        public static System.Action<int> OnLeftClick;
        public static System.Action<int> OnEnter;
        public static System.Action OnExit;

        int id;

        private void Start()
        {
            id = transform.GetSiblingIndex();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (OnLeftClick != null)
                    OnLeftClick(id);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // ������ʾ
            rect.color = new Color(0.8f, 0.8f, 0.8f);
            
            if (recordName.text != "�յ�")
            {
                if (OnEnter != null)
                    OnEnter(id);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // �ָ�������ɫ
            rect.color = Color.white;

            if (OnExit != null)
                OnExit();
        }

        // ���õ�λ��Ϣ
        public void SetName(int i)
        {
            indexText.text = i.ToString();
            
            //�յ�
            if (RecordData.Instance.recordName[i] == "")
            {
                recordName.text = "�յ�";
            }
            else
            {
                //��ȡ�浵��������
                string full = RecordData.Instance.recordName[i];
                //��ȡ����
                string date = full.Substring(0, 8);
                //��ȡʱ��
                string time = full.Substring(9, 6);
                //���ø�ʽ
                TimeMgr.SetDate(ref date);
                TimeMgr.SetTime(ref time);
                //��ʾ����
                recordName.text = date + " " + time;
            }
        }
    }
}