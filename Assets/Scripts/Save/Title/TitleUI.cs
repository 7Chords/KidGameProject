using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public Button Continue;       //继续游戏
    public Button Load;           //载入游戏
    public Button New;            //新游戏
    public Button Exit;           //退出游戏

    public GameObject recordPanel;

    private void Awake()
    {
        //读取最新存档
        Continue.onClick.AddListener(()=>LoadRecord(RecordData.Instance.lastID));
        //打开/关闭存档列表
        Load.onClick.AddListener(OpenRecordPanel);
        //存档被点击时调用
        OnlyLoad.OnLoad += LoadRecord;
        //新存档(所有数据初始化)
        New.onClick.AddListener(NewGame);
        //退出游戏(此处不存档)
        Exit.onClick.AddListener(QuitGame);
        
    }

    private void OnDestroy()
    {
        OnlyLoad.OnLoad -= LoadRecord;
    }

    private void Start()
    {
        //读取存档列表
        RecordData.Instance.Load();

        //有存档才激活按钮
        if (RecordData.Instance.lastID != 233)
        {
            Continue.interactable = true;
            Load.interactable = true;
        }           
    }

    void LoadRecord(int i)
    {
        //载入指定存档数据
        PlayerSaveData.Instance.Load(i);

        //如果最新存档不是i，就更新最新存档的序号，并保存
        if(i!= RecordData.Instance.lastID)
        {
            RecordData.Instance.lastID = i;
            RecordData.Instance.Save();
        }    
        
        //跳转场景
        SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
    }

    void OpenRecordPanel()
    {
        recordPanel.SetActive(!recordPanel.activeSelf);
    }

    void NewGame()
    {
        //初始化玩家数据
        //可以在Player里写个Init函数，也可以在预制体上直接设置

        //跳转至默认场景
        SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
    }

    void QuitGame()
    {     
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }



}
