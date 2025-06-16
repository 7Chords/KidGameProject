using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public Button Continue;       //������Ϸ
    public Button Load;           //������Ϸ
    public Button New;            //����Ϸ
    public Button Exit;           //�˳���Ϸ

    public GameObject recordPanel;

    private void Awake()
    {
        //��ȡ���´浵
        Continue.onClick.AddListener(()=>LoadRecord(RecordData.Instance.lastID));
        //��/�رմ浵�б�
        Load.onClick.AddListener(OpenRecordPanel);
        //�浵�����ʱ����
        OnlyLoad.OnLoad += LoadRecord;
        //�´浵(�������ݳ�ʼ��)
        New.onClick.AddListener(NewGame);
        //�˳���Ϸ(�˴����浵)
        Exit.onClick.AddListener(QuitGame);
        
    }

    private void OnDestroy()
    {
        OnlyLoad.OnLoad -= LoadRecord;
    }

    private void Start()
    {
        //��ȡ�浵�б�
        RecordData.Instance.Load();

        //�д浵�ż��ť
        if (RecordData.Instance.lastID != 233)
        {
            Continue.interactable = true;
            Load.interactable = true;
        }           
    }

    void LoadRecord(int i)
    {
        //����ָ���浵����
        PlayerSaveData.Instance.Load(i);

        //������´浵����i���͸������´浵����ţ�������
        if(i!= RecordData.Instance.lastID)
        {
            RecordData.Instance.lastID = i;
            RecordData.Instance.Save();
        }    
        
        //��ת����
        SceneManager.LoadScene(PlayerSaveData.Instance.scensName);
    }

    void OpenRecordPanel()
    {
        recordPanel.SetActive(!recordPanel.activeSelf);
    }

    void NewGame()
    {
        //��ʼ���������
        //������Player��д��Init������Ҳ������Ԥ������ֱ������

        //��ת��Ĭ�ϳ���
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
