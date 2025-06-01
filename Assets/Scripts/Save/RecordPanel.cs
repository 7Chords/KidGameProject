using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordPanel : MonoBehaviour
{
    public Transform grid;               //��λ������
    public GameObject recordPrefab;      //��λԤ����
    public GameObject recordPanel;      //�浵��塾������ʾ/���ء�

    [Header("��ť")]
    public Button open;
    public Button exit;
    public Button save;
    public Button load;
    [ColorUsage(true)]
    public Color oriColor;              //��ť��ʼ��ɫ
   
    [Header("�浵����")]   
    public GameObject detail;           //�浵����
    public Image screenShot;            //��ͼ
    public Text gameTime;               //ʱ��
    public Text sceneName;              //���ڳ���
    public Text level;                  //�ȼ�

    //Key���浵�ļ���     Value���浵���
    Dictionary<string, int> RecordInGrid = new Dictionary<string, int>();
    bool isSave=false;     //���ڴ浵
    bool isLoad=false;     //���ڶ���

    private void Start()
    {
        //����ָ�������浵λ
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj=Instantiate(recordPrefab, grid);
            //�����
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            //����õ�λ�д浵���͸�����Ĭ����Ϊ�յ�
            if (RecordData.Instance.recordName[i] != "")
            {
                obj.GetComponent<RecordUI>().SetName(i);               
                //�浽�ֵ���
                RecordInGrid.Add(RecordData.Instance.recordName[i], i);
            }            
        }

        #region ����
        RecordUI.OnLeftClick += LeftClickGrid;     
        RecordUI.OnRightClick += RightClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
        open.onClick.AddListener(() => CloseOrOpen());
        save.onClick.AddListener(()=>SaveOrLoad());
        load.onClick.AddListener(() => SaveOrLoad(false));
        exit.onClick.AddListener(QuitGame);
        #endregion

        //����ʱ��
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


    //RecordUI.OnEnter����
    void ShowDetails(int i)
    {
        //��ȡ�浵�������ı�������ݣ���������ʾ
        var data = Player.Instance.ReadForShow(i);
        gameTime.text = $"��Ϸʱ��  {TimeMgr.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"���ڳ���  {data.scensName}";
        level.text = $"��ҵȼ�  {data.level}";
        screenShot.sprite = SAVE.LoadShot(i);

        //��ʾ����
        detail.SetActive(true);
    }

    //RecordUI.OnExit����
    void HideDetails()
    {
        //��������
        detail.SetActive(false);
    }


    //��ťOPEN����
    void CloseOrOpen()
    {       
        //�޸���ʾ/����
        recordPanel.SetActive(!recordPanel.activeSelf);
        //�޸�����
        open.transform.GetChild(0).GetComponent<Text>().text = (recordPanel.activeSelf) ? "CLOSE" : "OPEN";
        //�޸Ŀɷ񻥶�
        save.interactable = (recordPanel.activeSelf) ? true : false;
        load.interactable = (recordPanel.activeSelf) ? true : false;
    }


    //��ťsave��load����
    void SaveOrLoad(bool OnSave=true)
    {
        //�޸�ģʽ
        isSave = OnSave;
        isLoad = !OnSave;
        //�޸���ɫ
        save.GetComponent<Image>().color = (isSave)?Color.white:oriColor;
        load.GetComponent<Image>().color = (isLoad)?Color.white:oriColor;
    }


    //���
    void LeftClickGrid(int ID)
    {
        //�浵
        if (isSave)
        {           
            NewRecord(ID);
        }
        //����
        else if (isLoad)
        {
            //�յ�ʲô������
            if (RecordData.Instance.recordName[ID] == "")           
                return;           
            else
            {
                //��ȡ�ô浵�������������
                Player.Instance.Load(ID);    
                //���µ�ǰ�浵ID�����浽�浵���ݿ�
                RecordData.Instance.lastID = ID;
                RecordData.Instance.Save();

                //��ת�������޸�ʱ��
                if (SceneManager.GetActiveScene().name != Player.Instance.scensName)
                {
                    SceneManager.LoadScene(Player.Instance.scensName);
                }
                TimeMgr.SetOriTime();
            }
        }
    }

    //�һ�ɾ��
    void RightClickGrid(int gridID)
    {
        if (RecordData.Instance.recordName[gridID] == "")        
            return;
        
        //�浵��Ϊ�վ�ɾ��
        else       
            DeleteRecord(gridID, false);
        
    }

    private void QuitGame()
    {
        string autoName = SAVE.FindAuto();
        if (autoName != "")
        {
            int autoID;
            //���Ҹ��Զ��浵���ֵ���ı��
            //�Ҳ�����ʱ��᷵��Ĭ��ֵ������int����0����������
            RecordInGrid.TryGetValue(autoName, out autoID);
            Debug.Log($"�����Զ��浵�����Ϊ{autoID}");
            //ɾ��ԭ�����Զ��浵������һ���µ��Զ��浵
            NewRecord(autoID, ".auto");
        }
        else
        {
            Debug.Log("���Զ��浵");
            for (int i = 0; i < RecordData.recordNum; i++)
            {
                //�ҿ�λ
                if (RecordData.Instance.recordName[i] == "")
                {
                    NewRecord(i, ".auto");
                    break;
                }
            }

        }

        //�˳���Ϸ
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }


    void NewRecord(int i,string end=".save")
    {
        //��λ��ԭ���д浵
        if (RecordData.Instance.recordName[i] != "")
        {
            //ֻɾ�ļ����ֵ�
            DeleteRecord(i);
        }

        //�½��浵��
        RecordData.Instance.recordName[i] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        //���µ�ǰ�浵��ţ�������´浵���浵�б�
        RecordData.Instance.lastID = i;
        RecordData.Instance.Save();
        //��������ݴ���ô浵�������ļ���
        Player.Instance.Save(i);
        //����´浵���ֵ�
        RecordInGrid.Add(RecordData.Instance.recordName[i], i);
        //���´浵UI
        grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
        //��ͼ
        SAVE.CameraCapture(i, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
        //��ʾ����
        ShowDetails(i);
    }


    //trueΪ����ģʽ��falseΪ��ɾ��ģʽ
    void DeleteRecord(int i,bool isCover = true)
    {
        //ɾ���浵�ļ�
        Player.Instance.Delete(i);
        //ɾ�ֵ�
        RecordInGrid.Remove(RecordData.Instance.recordName[i]);

        if (!isCover)
        {           
            //��մ浵��
            RecordData.Instance.recordName[i] = "";
            //����UI
            grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
            //ɾ����ͼ
            SAVE.DeleteShot(i);
            //��������
            HideDetails();
        }
    }
}
