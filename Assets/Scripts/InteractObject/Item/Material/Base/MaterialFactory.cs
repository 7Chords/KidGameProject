using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 材料工厂
    /// </summary>
    public static class MaterialFactory
    {
        public static GameObject CreateEntity(MaterialData materialData, Vector3 position)
        {
            //这里应该根据trapID加载对应的预制体
            //实际项目中应该使用资源管理系统

            GameObject materialPrefab = Resources.Load<GameObject>("Materials/" + materialData.materialName);
            if (materialPrefab == null)
            {
                Debug.LogWarning($"找不到材料预制体: {materialData.materialName}");
                return null;
            }

            GameObject materialObj = Object.Instantiate(materialPrefab, position, Quaternion.identity);
            MaterialBase material = materialObj.GetComponent<MaterialBase>();

            if (material == null)
            {
                Debug.LogWarning($"预制体 {materialData.materialName} 没有MaterialBase组件");
                Object.Destroy(materialObj);
                return null;
            }
            material.Init(materialData);
            return materialObj;
        }
    }
}
