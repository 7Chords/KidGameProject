using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace KidGame.Core
{
    public class SoLoader : MonoBehaviour
    {
        public static SoLoader Instance;
        public Dictionary<string, ScriptableObject> soDic;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else Destroy(this.gameObject);
            soDic = new Dictionary<string, ScriptableObject>();
            InitialSoResource();
        }




        private void InitialSoResource()
        {
            ScriptableObject[] allSobj = Resources.LoadAll<ScriptableObject>("ScriptObject");

            Debug.Log($"找到 {allSobj.Length} 个 ScriptableObject");
            // 添加到字典（键为文件名，不含扩展名）
            foreach (var so in allSobj)
            {
                if (so != null)
                {
                    if(!soDic.ContainsKey(so.name)) soDic.Add(so.name, so);
                    else Debug.LogWarning("重复加载！！ 检查是否有重名配表导致的重名SO！！");
                }
                else Debug.LogWarning("so为空！！");
            }
        }

        
    }

}
