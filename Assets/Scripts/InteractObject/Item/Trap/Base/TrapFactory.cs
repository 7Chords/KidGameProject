using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;


public static class TrapFactory
{
    public static TrapBase Create(string trapID, Vector3 position)
    {
        //����Ӧ�ø���trapID���ض�Ӧ��Ԥ����
        //ʵ����Ŀ��Ӧ��ʹ����Դ����ϵͳ
        
        GameObject trapPrefab = Resources.Load<GameObject>("Traps/" + trapID);
        if (trapPrefab == null)
        {
            Debug.LogWarning($"�Ҳ�������Ԥ����: {trapID}");
            return null;
        }
        
        GameObject trapObj = Object.Instantiate(trapPrefab, position, Quaternion.identity);
        TrapBase trap = trapObj.GetComponent<TrapBase>();
        
        if (trap == null)
        {
            Debug.LogWarning($"Ԥ���� {trapID} û��TrapBase���");
            Object.Destroy(trapObj);
            return null;
        }
        
        return trap;
    }
}