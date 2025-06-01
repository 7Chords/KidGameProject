using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordPanel : MonoBehaviour
{
    public Transform grid;               //档位父对象
    public GameObject recordPrefab;      //档位预制体
    public GameObject recordPanel;      //存档面板【控制显示/隐藏】

    [Header("按钮")]
    public Button open;
    public Button exit;
    public Button save;
    public Button load;
    [ColorUsage(true)]
    public Color oriColor;              //按钮初始颜色
   
    [Header("存档详情")]   
    public GameObject detail;           //存档详情
    public Image screenShot;            //截图
    public Text gameTime;               //时长
    public Text sceneName;              //所在场景
    public Text level;                  //等级

    //Key：存档文件名     Value：存档序号
    Dictionary<string, int> RecordInGrid = new Dictionary<string, int>();
    bool isSave=false;     //正在存档
    bool isLoad=false;     //正在读档

    private void Start()
    {
        //生成指定数量存档位
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj=Instantiate(recordPrefab, grid);
            //改序号
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            //如果该档位有存档，就改名，默认名为空档
            if (RecordData.Instance.recordName[i] != "")
            {
                obj.GetComponent<RecordUI>().SetName(i);               
                //存到字典里
                RecordInGrid.Add(RecordData.Instance.recordName[i], i);
            }            
        }

        #region 监听
        RecordUI.OnLeftClick += LeftClickGrid;     
        RecordUI.OnRightClick += RightClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
        open.onClick.AddListener(() => CloseOrOpen());
        save.onClick.AddListener(()=>SaveOrLoad());
        load.onClick.AddListener(() => SaveOrLoad(false));
        exit.onClick.AddListener(QuitGame);
        #endregion

        //设置时间
        TimeMgr.SetOriTime();
    }

    private void OnDestroy()
    {
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnRightClick -= RightClickGrid;
        RecordUI.OnEnter -= ShowDetails;
        RecordUI.OnExit -= HideDetails;
    }

    private void Update()
    {
        TimeMgr.SetCurTime();
    }


    //RecordUI.OnEnter调用
    void ShowDetails(int i)
    {
        //读取存档，但不改变玩家数据，仅用于显示
        var data = Player.Instance.ReadForShow(i);
        gameTime.text = $"游戏时长  {TimeMgr.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"所在场景  {data.scensName}";
        level.text = $"玩家等级  {data.level}";
        screenShot.sprite = SAVE.LoadShot(i);

        //显示详情
        detail.SetActive(true);
    }

    //RecordUI.OnExit调用
    void HideDetails()
    {
        //隐藏详情
        detail.SetActive(false);
    }


    //按钮OPEN调用
    void CloseOrOpen()
    {       
        //修改显示/隐藏
        recordPanel.SetActive(!recordPanel.activeSelf);
        //修改文字
        open.transform.GetChild(0).GetComponent<Text>().text = (recordPanel.activeSelf) ? "CLOSE" : "OPEN";
        //修改可否互动
        save.interactable = (recordPanel.activeSelf) ? true : false;
        load.interactable = (recordPanel.activeSelf) ? true : false;
    }


    //按钮save或load调用
    void SaveOrLoad(bool OnSave=true)
    {
        //修改模式
        isSave = OnSave;
        isLoad = !OnSave;
        //修改颜色
        save.GetComponent<Image>().color = (isSave)?Color.white:oriColor;
        load.GetComponent<Image>().color = (isLoad)?Color.white:oriColor;
    }


    //左击
    void LeftClickGrid(int ID)
    {
        //存档
        if (isSave)
        {           
            NewRecord(ID);
        }
        //读档
        else if (isLoad)
        {
            //空档什么都不做
            if (RecordData.Instance.recordName[ID] == "")           
                return;           
            else
            {
                //读取该存档，更新玩家数据
                Player.Instance.Load(ID);    
                //更新当前存档ID，保存到存档数据库
                RecordData.Instance.lastID = ID;
                RecordData.Instance.Save();

                //跳转场景并修改时间
                if (SceneManager.GetActiveScene().name != Player.Instance.scensName)
                {
                    SceneManager.LoadScene(Player.Instance.scensName);
                }
                TimeMgr.SetOriTime();
            }
        }
    }

    //右击删除
    void RightClickGrid(int gridID)
    {
        if (RecordData.Instance.recordName[gridID] == "")        
            return;
        
        //存档不为空就删除
        else       
            DeleteRecord(gridID, false);
        
    }

    private void QuitGame()
    {
        string autoName = SAVE.FindAuto();
        if (autoName != "")
        {
            int autoID;
            //查找该自动存档在字典里的编号
            //找不到的时候会返回默认值（比如int就是0），不报错
            RecordInGrid.TryGetValue(autoName, out autoID);
            Debug.Log($"已有自动存档，序号为{autoID}");
            //删除原来的自动存档，创建一个新的自动存档
            NewRecord(autoID, ".auto");
        }
        else
        {
            Debug.Log("无自动存档");
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                //找空位
                if (RecordData.Instance.recordName[i] == "")
                {
                    NewRecord(i, ".auto");
                    break;
                }
            }

        }

        //退出游戏
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }


    void NewRecord(int i,string end=".save")
    {
        //该位置原来有存档
        if (RecordData.Instance.recordName[i] != "")
        {
            //只删文件和字典
            DeleteRecord(i);
        }

        //新建存档名
        RecordData.Instance.recordName[i] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        //更新当前存档编号，保存该新存档到存档列表
        RecordData.Instance.lastID = i;
        RecordData.Instance.Save();
        //将玩家数据存入该存档【创建文件】
        Player.Instance.Save(i);
        //添加新存档到字典
        RecordInGrid.Add(RecordData.Instance.recordName[i], i);
        //更新存档UI
        grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
        //截图
        SAVE.CameraCapture(i, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
        //显示详情
        ShowDetails(i);
    }


    //true为覆盖模式，false为纯删除模式
    void DeleteRecord(int i,bool isCover = true)
    {
        //删除存档文件
        Player.Instance.Delete(i);
        //删字典
        RecordInGrid.Remove(RecordData.Instance.recordName[i]);

        if (!isCover)
        {           
            //清空存档名
            RecordData.Instance.recordName[i] = "";
            //更新UI
            grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
            //删除截图
            SAVE.DeleteShot(i);
            //隐藏详情
            HideDetails();
        }
    }
}
