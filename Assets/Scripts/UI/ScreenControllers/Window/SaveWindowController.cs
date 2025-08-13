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

        private int currentSelectedID = -1; //当前选中的档位ID

        private void Start()
        {
            //生成指定数量档位   
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                GameObject obj = Instantiate(recordPrefab, grid);
                obj.name = (i + 1).ToString();
                obj.GetComponent<SaveCell>().SetName(i + 1);
                //如果该档位有存档，就改名，默认名为空档
                if (RecordData.Instance.recordName[i] != "")
                    obj.GetComponent<SaveCell>().SetName(i);
            }

            SaveCell.OnLeftClick += LeftClickGrid;
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
        }
        

        // 读取按钮点击事件
        void OnLoadButtonClick()
        {
            if (currentSelectedID != -1)
            {
                LoadRecord(currentSelectedID);
            }
        }
        
        void LeftClickGrid(int gridID)
        {
            currentSelectedID = gridID;
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