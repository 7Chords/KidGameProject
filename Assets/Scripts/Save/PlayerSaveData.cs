using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��������ϣ�д��Ҫ�������Ϣ
    /// </summary>
    public class PlayerSaveData : SingletonPersistent<PlayerSaveData>
    {
        public int level; //��ҵȼ�
        public string scensName; //����ʱ���ڳ�����  
        public float gameTime; //��Ϸʱ��
        public bool isFullScreen; //�Ƿ�ȫ��


        public class SaveData
        {
            public string scensName;
            public int level;
            public float gameTime;
            public bool isFullScreen;
            public Color color;
        }

        SaveData ForSave()
        {
            var savedata = new SaveData();
            savedata.scensName = scensName;
            savedata.level = level;
            savedata.gameTime = gameTime;
            savedata.isFullScreen = isFullScreen;
            return savedata;
        }

        void ForLoad(SaveData savedata)
        {
            scensName = savedata.scensName;
            level = savedata.level;
            gameTime = savedata.gameTime;
            isFullScreen = savedata.isFullScreen;
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
}