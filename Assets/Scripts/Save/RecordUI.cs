using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidGame.Core
{
    public class RecordUI : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public Text indexText;         //序号
        public Text recordName;        //存档名
        public GameObject auto;        //自动存档的标识
        public Image rect;             //边框
        [ColorUsage(true)]
        public Color enterColor;       //鼠标进入存档时的边框颜色
    

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
            //鼠标进入边框变色
            rect.color = enterColor;

            //有存档则显示详情（根据ID读取该存档数据，即SiblingIndex）
            if (recordName.text != "空档")
            {
                if (OnEnter != null)
                    OnEnter(id);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //鼠标退出边框变色
            rect.color = Color.white;

            //隐藏详情
            if (OnExit != null)
                OnExit();       
        }

        //初始化存档列表时设置序号
        public void SetID(int i)
        {
            indexText.text = i.ToString();
        }


        public void SetName(int i)
        {
            //空档，隐藏Auto标识（因为有可能是删档时调用的）
            if (RecordData.Instance.recordName[i] == "")
            {
                recordName.text = "空档";
                auto.SetActive(false);
            }
            else
            {
                //获取存档文件名【完整,带后缀】
                string full = RecordData.Instance.recordName[i];
                //截取日期【8位】
                string date = full.Substring(0, 8);
                //截取时间【6位】
                string time = full.Substring(9, 6);            
                //设置格式
                TimeMgr.SetDate(ref date);
                TimeMgr.SetTime(ref time);
                //输出显示
                recordName.text = date + " " + time;

                //根据存档类型设置Auto标识
                if (full.Substring(full.Length - 4) == "auto")
                    auto.SetActive(true);                
                else
                    auto.SetActive(false);            
            }
        
        }

    
    }
}
