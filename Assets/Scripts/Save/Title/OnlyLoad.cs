using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.UI;


//只有加载功能的存档面板，用于标题场景
public class OnlyLoad : MonoBehaviour
{
    public Transform grid; //档位父对象
    public GameObject recordPrefab; //档位预制体

    [Header("存档详情")] public GameObject detail; //存档详情
    public Image screenShot; //截图
    public Text gameTime; //时长
    public Text sceneName; //所在场景
    public Text level; //等级

    //存档被点击时执行
    public static System.Action<int> OnLoad;

    private void Start()
    {
        //生成指定数量档位   
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj = Instantiate(recordPrefab, grid);
            //改序号
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            //如果该档位有存档，就改名，默认名为空档
            if (RecordData.Instance.recordName[i] != "")
                obj.GetComponent<RecordUI>().SetName(i);
        }


        RecordUI.OnLeftClick += LeftClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
    }

    private void OnDestroy()
    {
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnEnter -= ShowDetails;
        RecordUI.OnExit -= HideDetails;
    }

    //RecordUI.OnEnter调用
    void ShowDetails(int i)
    {
        //读取存档，但不修改玩家数据，仅用于显示
        var data = PlayerSaveData.Instance.ReadForShow(i);
        screenShot.sprite = SAVE.LoadShot(i);
        gameTime.text = $"游戏时长  {TimeMgr.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"所在场景  {data.scensName}";
        level.text = $"玩家等级  {data.level}";

        //显示详情
        detail.SetActive(true);
    }

    //RecordUI.OnExit调用
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
            if (OnLoad != null)
                OnLoad(gridID);
        }
    }
}