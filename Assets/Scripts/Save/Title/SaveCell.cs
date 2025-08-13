using KidGame.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace KidGame.UI
{
    public class SaveCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text indexText; //序号
        public Text recordName; //存档名
        public Image rect; //背景

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
            // 高亮显示
            rect.color = new Color(0.8f, 0.8f, 0.8f);
            
            if (recordName.text != "空档")
            {
                if (OnEnter != null)
                    OnEnter(id);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 恢复背景颜色
            rect.color = Color.white;

            if (OnExit != null)
                OnExit();
        }

        // 设置档位信息
        public void SetName(int i)
        {
            indexText.text = i.ToString();
            
            //空档
            if (RecordData.Instance.recordName[i] == "")
            {
                recordName.text = "空档";
            }
            else
            {
                //获取存档完整名称
                string full = RecordData.Instance.recordName[i];
                //获取日期
                string date = full.Substring(0, 8);
                //获取时间
                string time = full.Substring(9, 6);
                //设置格式
                TimeMgr.SetDate(ref date);
                TimeMgr.SetTime(ref time);
                //显示名称
                recordName.text = date + " " + time;
            }
        }
    }
}