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

            Debug.Log($"�ҵ� {allSobj.Length} �� ScriptableObject");
            // ��ӵ��ֵ䣨��Ϊ�ļ�����������չ����
            foreach (var so in allSobj)
            {
                if (so != null)
                {
                    if(!soDic.ContainsKey(so.name)) soDic.Add(so.name, so);
                    else Debug.LogWarning("�ظ����أ��� ����Ƿ�����������µ�����SO����");
                }
                else Debug.LogWarning("soΪ�գ���");
            }
        }

        
    }

}
