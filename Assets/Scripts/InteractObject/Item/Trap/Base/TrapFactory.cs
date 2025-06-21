using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ���幤��
    /// </summary>
    public static class TrapFactory
    {
        public static GameObject Create(TrapData trapData, Vector3 position)
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
            trap.Init(trapData);
            if (trap == null)
            {
                Debug.LogWarning($"Ԥ���� {trapData.trapID} û��TrapBase���");
                Object.Destroy(trapObj);
                return null;
            }

            return trapObj;
        }
    }
}