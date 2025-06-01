using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ����

    public static Player Instance;

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


    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public int level; //��ҵȼ�
    public string scensName; //����ʱ���ڳ�����  
    public float gameTime; //��Ϸʱ��
    public bool isFullScreen; //�Ƿ�ȫ��
    public Difficulty difficulty; //�Ѷ�
    [ColorUsage(true)] public Color color; //ģ����ɫ


    public class SaveData
    {
        public string scensName;
        public int level;
        public float gameTime;
        public bool isFullScreen;
        public Color color;
        public Difficulty difficulty;
    }

    SaveData ForSave()
    {
        var savedata = new SaveData();
        savedata.scensName = scensName;
        savedata.level = level;
        savedata.gameTime = gameTime;
        savedata.isFullScreen = isFullScreen;
        savedata.color = color;
        savedata.difficulty = difficulty;
        return savedata;
    }

    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        level = savedata.level;
        gameTime = savedata.gameTime;
        isFullScreen = savedata.isFullScreen;
        color = savedata.color;
        difficulty = savedata.difficulty;
    }

    public void Save(int id)
    {
        SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
    }

    public void Load(int id)
    {
        var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        ForLoad(saveData);
    }

    public SaveData ReadForShow(int id)
    {
        return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
    }

    public void Delete(int id)
    {
        SAVE.JsonDelete(RecordData.Instance.recordName[id]);
    }
}