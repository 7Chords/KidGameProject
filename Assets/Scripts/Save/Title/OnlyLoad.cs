using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.UI;


//ֻ�м��ع��ܵĴ浵��壬���ڱ��ⳡ��
public class OnlyLoad : MonoBehaviour
{
    public Transform grid; //��λ������
    public GameObject recordPrefab; //��λԤ����

    [Header("�浵����")] public GameObject detail; //�浵����
    public Image screenShot; //��ͼ
    public Text gameTime; //ʱ��
    public Text sceneName; //���ڳ���
    public Text level; //�ȼ�

    //�浵�����ʱִ��
    public static System.Action<int> OnLoad;

    private void Start()
    {
        //����ָ��������λ   
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj = Instantiate(recordPrefab, grid);
            //�����
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            //����õ�λ�д浵���͸�����Ĭ����Ϊ�յ�
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

    //RecordUI.OnEnter����
    void ShowDetails(int i)
    {
        //��ȡ�浵�������޸�������ݣ���������ʾ
        var data = PlayerSaveData.Instance.ReadForShow(i);
        screenShot.sprite = SAVE.LoadShot(i);
        gameTime.text = $"��Ϸʱ��  {TimeMgr.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"���ڳ���  {data.scensName}";
        level.text = $"��ҵȼ�  {data.level}";

        //��ʾ����
        detail.SetActive(true);
    }

    //RecordUI.OnExit����
    void HideDetails()
    {
        //��������
        detail.SetActive(false);
    }

    //������ش浵
    void LeftClickGrid(int gridID)
    {
        //�յ�ʲô������
        if (RecordData.Instance.recordName[gridID] == "")
            return;
        else
        {
            if (OnLoad != null)
                OnLoad(gridID);
        }
    }
}