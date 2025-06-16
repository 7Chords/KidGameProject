using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidGame.Core
{
    public class RecordUI : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public Text indexText;         //���
        public Text recordName;        //�浵��
        public GameObject auto;        //�Զ��浵�ı�ʶ
        public Image rect;             //�߿�
        [ColorUsage(true)]
        public Color enterColor;       //������浵ʱ�ı߿���ɫ
    

        public static System.Action<int> OnLeftClick;
        public static System.Action<int> OnRightClick;
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

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (OnRightClick != null)
                    OnRightClick(id);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //������߿��ɫ
            rect.color = enterColor;

            //�д浵����ʾ���飨����ID��ȡ�ô浵���ݣ���SiblingIndex��
            if (recordName.text != "�յ�")
            {
                if (OnEnter != null)
                    OnEnter(id);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //����˳��߿��ɫ
            rect.color = Color.white;

            //��������
            if (OnExit != null)
                OnExit();       
        }

        //��ʼ���浵�б�ʱ�������
        public void SetID(int i)
        {
            indexText.text = i.ToString();
        }


        public void SetName(int i)
        {
            //�յ�������Auto��ʶ����Ϊ�п�����ɾ��ʱ���õģ�
            if (RecordData.Instance.recordName[i] == "")
            {
                recordName.text = "�յ�";
                auto.SetActive(false);
            }
            else
            {
                //��ȡ�浵�ļ���������,����׺��
                string full = RecordData.Instance.recordName[i];
                //��ȡ���ڡ�8λ��
                string date = full.Substring(0, 8);
                //��ȡʱ�䡾6λ��
                string time = full.Substring(9, 6);            
                //���ø�ʽ
                TimeMgr.SetDate(ref date);
                TimeMgr.SetTime(ref time);
                //�����ʾ
                recordName.text = date + " " + time;

                //���ݴ浵��������Auto��ʶ
                if (full.Substring(full.Length - 4) == "auto")
                    auto.SetActive(true);                
                else
                    auto.SetActive(false);            
            }
        
        }

    
    }
}
