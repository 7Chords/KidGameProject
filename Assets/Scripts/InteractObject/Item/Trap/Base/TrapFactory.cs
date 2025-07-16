using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ���幤��
    /// </summary>
    public static class TrapFactory
    {
        public static GameObject CreateEntity(TrapData trapData, Vector3 position)
        {
            //����Ӧ�ø���trapID���ض�Ӧ��Ԥ����
            //ʵ����Ŀ��Ӧ��ʹ����Դ����ϵͳ
            GameObject trapPrefab = Resources.Load<GameObject>("Traps/" + trapData.trapName);
            if (trapPrefab == null)
            {
                Debug.LogWarning($"�Ҳ�������Ԥ����: {trapData.trapName}");
                return null;
            }

            GameObject trapObj = Object.Instantiate(trapPrefab, position, Quaternion.identity);
            TrapBase trap = trapObj.GetComponent<TrapBase>();
            if (trap == null)
            {
                Debug.LogWarning($"Ԥ���� {trapData.trapID} û��TrapBase���");
                Object.Destroy(trapObj);
                return null;
            }
            trap.Init(trapData);

            return trapObj;
        }

        public static GameObject CreatePreview(TrapData trapData, Vector3 position,Transform parent)
        {
            GameObject trapPreview = Resources.Load<GameObject>("Traps/" + trapData.trapName);
            if (trapPreview == null)
            {
                Debug.LogWarning($"�Ҳ�������Ԥ����: {trapData.trapName}");
                return null;
            }

            GameObject trapObj = Object.Instantiate(trapPreview, position, Quaternion.identity);
            TrapBase trap = trapObj.GetComponent<TrapBase>();
            if (trap == null)
            {
                Debug.LogWarning($"Ԥ���� {trapData.trapID} û��TrapBase���");
                Object.Destroy(trapObj);
                return null;
            }
            trapObj.transform.SetParent(parent);
            trap.ShowPlacePreview();
            return trapObj;
        }
    }
}