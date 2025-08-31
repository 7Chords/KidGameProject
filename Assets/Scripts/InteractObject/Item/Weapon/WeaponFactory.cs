using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    public static class WeaponFactory
    {
        public static GameObject CreateEntity(WeaponData weaponData, Vector3 position, Transform parent)
        {
            GameObject weaponPrefab = Resources.Load<GameObject>("Weapon/" + weaponData.name);
            if (weaponPrefab == null)
            {
                Debug.LogWarning($"找不到武器预制体: {weaponData.name}");
                return null;
            }
            WeaponBase weapon = null;
            GameObject weaponObj = Object.Instantiate(weaponPrefab
                , position
                , Quaternion.identity
                , parent);

            if (weaponObj != null)
                weapon = weaponObj.GetComponent<WeaponBase>();
            else Debug.LogError("weaponObj == NULL!!!");

            if (weapon == null)
            {
                Debug.LogWarning($"预制体 {weaponData.id} 没有TrapBase组件");
                Object.Destroy(weaponObj);
                return null;
            }
            PlayerController.Instance.SetCurrentWeaponData(weaponData);
            weapon.InitWeaponData(weaponData);// 初始化读配表数据
            weapon.SetIsNotUse(true); // 初始化为还未使用
            return weaponObj;
        }

    }

}
