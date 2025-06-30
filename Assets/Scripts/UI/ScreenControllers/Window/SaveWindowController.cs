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

        [Header("存档详情")] public GameObject detail; //存档详情
        public Text gameTime; //时长
        public Text sceneName; //所在场景

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
        }

        private void OnDestroy()
        {
            SaveCell.OnLeftClick -= LeftClickGrid;
            SaveCell.OnEnter -= ShowDetails;
            SaveCell.OnExit -= HideDetails;
        }

        void ShowDetails(int i)
        {
            //读取存档，但不修改玩家数据，仅用于显示
            var data = PlayerSaveData.Instance.ReadForShow(i);
            gameTime.text = $"游戏时长  {TimeMgr.GetFormatTime((int)data.gameTime)}";
            sceneName.text = $"所在场景  {data.scensName}";

            detail.SetActive(true);
        }

        void HideDetails()
        {
            //隐藏详情
            detail.SetActive(false);
        }

        //左击加载存档
        void LeftClickGrid(int gridID)
        {
            //空档什么都不做
            if (RecordData.Instance.recordName[gridID] == "")
                return;
            else
            {
                LoadRecord(gridID);
            }
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

            //跳转场景
            SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
        }
    }
}