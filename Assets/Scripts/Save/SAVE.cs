using System.IO;
using UnityEngine;

namespace KidGame.Core
{
    public static class SAVE
    {
        //截图保存路径
        public static string shotPath = $"{Application.persistentDataPath}/Shot";

        static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        #region  PlayerPrefs
        public static void PlayerPrefsSave(string key, object data)
        {
            //将各种类型数据转为字符串存储
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public static string PlayerPrefsLoad(string key)
        {
            //参数2是没有数据时的默认值（不一定要null）
            return PlayerPrefs.GetString(key, null);
        }
   
        #endregion

        #region JSON
        public static void JsonSave(string fileName, object data)
        {
            string json = JsonUtility.ToJson(data);

            File.WriteAllText(GetPath(fileName), json);
            Debug.Log($"已保存{GetPath(fileName)}");
    
        }

        public static T JsonLoad<T>(string fileName)
        {
            string path = GetPath(fileName);
            //文件存在就读取
            if (File.Exists(path))
            {
                string json = File.ReadAllText(GetPath(fileName));
                var data = JsonUtility.FromJson<T>(json);
                Debug.Log($"读取{path}");
                return data;
            }
            else
            {
                //对于引用类型会返回null，对于数值类型会返回0
                return default;
            }
        }

        public static void JsonDelete(string fileName)
        {
            File.Delete(GetPath(fileName));
        }

        public static string FindAuto()
        {
            //确认路径存在
            if (Directory.Exists(Application.persistentDataPath))
            {
                //获取所有存档文件
                FileInfo[] fileInfos = new DirectoryInfo(Application.persistentDataPath).GetFiles("*");
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    //找自动存档
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

        #region 截图
        /*方法一：全屏、带UI   
    不写路径的话默认保存到项目文件夹下（与Asset同级）
    如果已存在会直接覆盖
    ScreenCapture.CaptureScreenshot(path);
    */

        /*方法二：指定相机的指定范围
    */
        public static void CameraCapture(int i, Camera camera, Rect rect)
        {
            //不存在文件夹就新建
            if (!Directory.Exists(SAVE.shotPath))
                Directory.CreateDirectory(SAVE.shotPath);
            string path = Path.Combine(SAVE.shotPath, $"{i}.png");

            int w = (int)rect.width;
            int h = (int)rect.height;

            RenderTexture rt = new RenderTexture(w, h, 0);
            //将相机渲染的内容存到指定的RenderTexture
            camera.targetTexture = rt;
            camera.Render();

            ////多相机测试
            //Camera c2 = camera.GetUniversalAdditionalCameraData().cameraStack[0];
            //c2.targetTexture=rt;
            //c2.Render();


            //激活指定RenderTexture
            RenderTexture.active = rt;

            //参数4：mipChain多级渐远纹理
            Texture2D t2D = new Texture2D(w, h, TextureFormat.RGB24, true);

            //防止截黑屏,但可能会导致截错(?)
            //yield return new WaitForEndOfFrame();
            //把RenderTexture的像素读到Texture2D
            t2D.ReadPixels(rect, 0, 0);
            t2D.Apply();

            //存成PNG
            byte[] bytes = t2D.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            //用完重置、销毁    
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
                Debug.Log($"删除截图{i}");
            }
        }

        #endregion

        #region 清空
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Delete/Records List")]
        public static void DeleteRecord()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            Debug.Log("已清空存档列表");
        }

        [UnityEditor.MenuItem("Delete/Player Data")]
        public static void DeletePlayerData()
        {
            ClearDirectory(Application.persistentDataPath);
            Debug.Log("已清空玩家数据");
        }

        static void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                FileInfo[] f = new DirectoryInfo(path).GetFiles("*");
                for (int i = 0; i < f.Length; i++)
                {
                    Debug.Log($"删除{f[i].Name}");
                    File.Delete(f[i].FullName);
                }
            }
        }

        [UnityEditor.MenuItem("Delete/Shot")]
        public static void DeleteScreenShot()
        {
            ClearDirectory(shotPath);
            Debug.Log("已清空截图");
        }
        
        [UnityEditor.MenuItem("Delete/Global Data")]
        public static void DeleteGlobalData()
        {
            ResetGlobalData();
            Debug.Log("已清空全局数据");
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
