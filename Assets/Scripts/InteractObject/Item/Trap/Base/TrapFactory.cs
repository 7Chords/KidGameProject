using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 陷阱工厂
    /// </summary>
    public static class TrapFactory
    {
        public static GameObject Create(TrapData trapData, Vector3 position)
        {
            //这里应该根据trapID加载对应的预制体
            //实际项目中应该使用资源管理系统

            GameObject trapPrefab = Resources.Load<GameObject>("Traps/" + trapData.trapName);
            if (trapPrefab == null)
            {
                Debug.LogWarning($"找不到陷阱预制体: {trapData.trapName}");
                return null;
            }

            GameObject trapObj = Object.Instantiate(trapPrefab, position, Quaternion.identity);
            TrapBase trap = trapObj.GetComponent<TrapBase>();
            trap.Init(trapData);
            if (trap == null)
            {
                Debug.LogWarning($"预制体 {trapData.trapID} 没有TrapBase组件");
                Object.Destroy(trapObj);
                return null;
            }

            return trapObj;
        }
    }
}