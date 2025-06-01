using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordData : MonoBehaviour
{
    #region ����
    public static RecordData Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

    }
    #endregion

    public const int recordNum = 20;             //��λ��
    public const string NAME = "RecordData";     //�浵�б���

    public string[] recordName = new string[recordNum];    //�浵�ļ���(����ȫ·����)
    public int lastID;                                     //���´浵���(��������ʱ�Զ�����)

    class SaveData
    {
        public string[] recordName = new string[recordNum];
        public int lastID;
    }

    SaveData ForSave()
    {
        var savedata = new SaveData();
      
        for (int i = 0; i < recordNum; i++)
        {
            savedata.recordName[i] = recordName[i];
        }
        savedata.lastID = lastID;

        return savedata;
    }

    void ForLoad(SaveData savedata)
    {       
        lastID = savedata.lastID;
        for (int i = 0; i < recordNum; i++)
        {
            recordName[i] = savedata.recordName[i];
        }
    }

    public void Save()
    {
        SAVE.PlayerPrefsSave(NAME, ForSave());
    }

    public void Load()
    {
        //�д浵�Ŷ�
        if (PlayerPrefs.HasKey(NAME))
        {
            string json = SAVE.PlayerPrefsLoad(NAME);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            ForLoad(saveData);
        }
    }

    
}
