using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;


public static class TrapFactory
{
    public static TrapBase Create(string trapID, Vector3 position)
    {
        //这里应该根据trapID加载对应的预制体
        //实际项目中应该使用资源管理系统
        
        GameObject trapPrefab = Resources.Load<GameObject>("Traps/" + trapID);
        if (trapPrefab == null)
        {
            Debug.LogWarning($"找不到陷阱预制体: {trapID}");
            return null;
        }
        
        GameObject trapObj = Object.Instantiate(trapPrefab, position, Quaternion.identity);
        TrapBase trap = trapObj.GetComponent<TrapBase>();
        
        if (trap == null)
        {
            Debug.LogWarning($"预制体 {trapID} 没有TrapBase组件");
            Object.Destroy(trapObj);
            return null;
        }
        
        return trap;
    }
}