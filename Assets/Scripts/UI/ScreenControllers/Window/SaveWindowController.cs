using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace KidGame.UI
{
    public class SaveWindowController : WindowController
    {
        public Transform grid; //档位父对象
        public GameObject recordPrefab; //档位预制体

        [Header("存档详情")] 
        public GameObject detail; //存档详情
        public Text gameTime; //时长
        public Text sceneName; //所在场景
        public Button loadButton; //读取按钮
        public Button deleteButton; //删除按钮

        private int currentSelectedID = -1; //当前选中的档位ID

        private void Start()
        {
            //生成指定数量档位   
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                obj.name = (i + 1).ToString();
                obj.GetComponent<SaveCell>().SetID(i + 1);
                //如果该档位有存档，就改名，默认名为空档
                if (RecordData.Instance.recordName[i] != "")
                    obj.GetComponent<SaveCell>().SetName(i);
            }

            SaveCell.OnLeftClick += LeftClickGrid;
            SaveCell.OnEnter += ShowDetails;
            SaveCell.OnExit += HideDetails;
            
            loadButton.onClick.AddListener(OnLoadButtonClick);
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
            SaveCell.OnEnter -= ShowDetails;
            SaveCell.OnExit -= HideDetails;
            
            loadButton.onClick.RemoveListener(OnLoadButtonClick);
            deleteButton.onClick.RemoveListener(OnDeleteButtonClick);
        }

        void ShowDetails(int i)
        {
            currentSelectedID = i;
            
            var data = PlayerSaveData.Instance.ReadForShow(i);
            gameTime.text = $"游戏时长  {TimeMgr.GetFormatTime((int)data.gameTime)}";
            sceneName.text = $"所在场景  {data.scensName}";

            // 只有有存档时才显示按钮
            bool hasSave = RecordData.Instance.recordName[i] != "";
            loadButton.gameObject.SetActive(hasSave);
            deleteButton.gameObject.SetActive(hasSave);

            detail.SetActive(true);
        }

        void HideDetails()
        {
            currentSelectedID = -1;
            detail.SetActive(false);
        }

        // 读取按钮点击事件
        void OnLoadButtonClick()
        {
            if (currentSelectedID != -1)
            {
                LoadRecord(currentSelectedID);
            }
        }

        // 删除按钮点击事件
        void OnDeleteButtonClick()
        {
            if (currentSelectedID != -1)
            {
                // 删除存档
                RecordData.Instance.recordName[currentSelectedID] = "";
                PlayerSaveData.Instance.Delete(currentSelectedID);
                
                // 更新UI
                Transform cell = grid.GetChild(currentSelectedID);
                cell.GetComponent<SaveCell>().SetName(currentSelectedID);
                
                // 隐藏详情和按钮
                detail.SetActive(false);
                currentSelectedID = -1;
            }
        }
        
        void LeftClickGrid(int gridID)
        {
            currentSelectedID = gridID;
            ShowDetails(gridID);
        }

        void LoadRecord(int i)
        {
            //载入指定存档数据
            PlayerSaveData.Instance.Load(i);

            //如果最新存档不是i，就更新最新存档的序号，并保存
            if (i != RecordData.Instance.lastID)
            {
                RecordData.Instance.lastID = i;
                RecordData.Instance.Save();
            }
            
            PlayerSaveData.Instance.currentSaveSlot = i;

            //跳转场景
            SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
        }
    }
}