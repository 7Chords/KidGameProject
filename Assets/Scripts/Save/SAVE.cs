using System.IO;
using UnityEngine;

namespace KidGame.Core
{
    public static class SAVE
    {
        //��ͼ����·��
        public static string shotPath = $"{Application.persistentDataPath}/Shot";

        static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        #region  PlayerPrefs
        public static void PlayerPrefsSave(string key, object data)
        {
            //��������������תΪ�ַ����洢
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public static string PlayerPrefsLoad(string key)
        {
            //����2��û������ʱ��Ĭ��ֵ����һ��Ҫnull��
            return PlayerPrefs.GetString(key, null);
        }
   
        #endregion

        #region JSON
        public static void JsonSave(string fileName, object data)
        {
            string json = JsonUtility.ToJson(data);

            File.WriteAllText(GetPath(fileName), json);
            Debug.Log($"�ѱ���{GetPath(fileName)}");
    
        }

        public static T JsonLoad<T>(string fileName)
        {
            string path = GetPath(fileName);
            //�ļ����ھͶ�ȡ
            if (File.Exists(path))
            {
                string json = File.ReadAllText(GetPath(fileName));
                var data = JsonUtility.FromJson<T>(json);
                Debug.Log($"��ȡ{path}");
                return data;
            }
            else
            {
                //�����������ͻ᷵��null��������ֵ���ͻ᷵��0
                return default;
            }
        }

        public static void JsonDelete(string fileName)
        {
            File.Delete(GetPath(fileName));
        }

        public static string FindAuto()
        {
            //ȷ��·������
            if (Directory.Exists(Application.persistentDataPath))
            {
                //��ȡ���д浵�ļ�
                FileInfo[] fileInfos = new DirectoryInfo(Application.persistentDataPath).GetFiles("*");
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    //���Զ��浵
                    if (fileInfos[i].Name.EndsWith(".auto"))
                    {
                        return fileInfos[i].Name;
                    }
                }
            }
            return "";
        }
        
        public static void ResetGlobalData()
        {
            RecordData.Instance.unlockedItems.Clear();
            RecordData.Instance.Save();
        }
        
        #endregion

        #region ��ͼ
        /*����һ��ȫ������UI   
    ��д·���Ļ�Ĭ�ϱ��浽��Ŀ�ļ����£���Assetͬ����
    ����Ѵ��ڻ�ֱ�Ӹ���
    ScreenCapture.CaptureScreenshot(path);
    */

        /*��������ָ�������ָ����Χ
    */
        public static void CameraCapture(int i, Camera camera, Rect rect)
        {
            //�������ļ��о��½�
            if (!Directory.Exists(SAVE.shotPath))
                Directory.CreateDirectory(SAVE.shotPath);
            string path = Path.Combine(SAVE.shotPath, $"{i}.png");

            int w = (int)rect.width;
            int h = (int)rect.height;

            RenderTexture rt = new RenderTexture(w, h, 0);
            //�������Ⱦ�����ݴ浽ָ����RenderTexture
            camera.targetTexture = rt;
            camera.Render();

            ////���������
            //Camera c2 = camera.GetUniversalAdditionalCameraData().cameraStack[0];
            //c2.targetTexture=rt;
            //c2.Render();


            //����ָ��RenderTexture
            RenderTexture.active = rt;

            //����4��mipChain�༶��Զ����
            Texture2D t2D = new Texture2D(w, h, TextureFormat.RGB24, true);

            //��ֹ�غ���,�����ܻᵼ�½ش�(?)
            //yield return new WaitForEndOfFrame();
            //��RenderTexture�����ض���Texture2D
            t2D.ReadPixels(rect, 0, 0);
            t2D.Apply();

            //���PNG
            byte[] bytes = t2D.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            //�������á�����    
            camera.targetTexture = null;
            //c2.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);
        }


        public static Sprite LoadShot(int i)
        {
            var path = Path.Combine(shotPath, $"{i}.png");

            Texture2D t = new Texture2D(640, 360);
            t.LoadImage(GetImgByte(path));
            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }


        static byte[] GetImgByte(string path)
        {
            FileStream s = new FileStream(path, FileMode.Open);
            byte[] imgByte = new byte[s.Length];
            s.Read(imgByte, 0, imgByte.Length);
            s.Close();
            return imgByte;
        }

        public static void DeleteShot(int i)
        {
            var path = Path.Combine(shotPath, $"{i}.png");
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"ɾ����ͼ{i}");
            }
        }

        #endregion

        #region ���
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Delete/Records List")]
        public static void DeleteRecord()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            Debug.Log("����մ浵�б�");
        }

        [UnityEditor.MenuItem("Delete/Player Data")]
        public static void DeletePlayerData()
        {
            ClearDirectory(Application.persistentDataPath);
            Debug.Log("������������");
        }

        static void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                FileInfo[] f = new DirectoryInfo(path).GetFiles("*");
                for (int i = 0; i < f.Length; i++)
                {
                    Debug.Log($"ɾ��{f[i].Name}");
                    File.Delete(f[i].FullName);
                }
            }
        }

        [UnityEditor.MenuItem("Delete/Shot")]
        public static void DeleteScreenShot()
        {
            ClearDirectory(shotPath);
            Debug.Log("����ս�ͼ");
        }
        
        [UnityEditor.MenuItem("Delete/Global Data")]
        public static void DeleteGlobalData()
        {
            ResetGlobalData();
            Debug.Log("�����ȫ������");
        }

        [UnityEditor.MenuItem("Delete/All")]
        public static void DeleteAll()
        {
            DeletePlayerData();
            DeleteRecord();
            DeleteScreenShot();
            DeleteGlobalData();
        }
#endif
        #endregion

    }
}
