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
                Debug.LogWarning($"�Ҳ�������Ԥ����: {weaponData.name}");
                return null;
            }
            WeaponBase weapon = null;
            GameObject weaponObj = Object.Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity);
            if (weaponObj != null)
                weapon = weaponObj.GetComponent<WeaponBase>();
            else Debug.LogError("weaponObj == NULL!!!");

            if (weapon == null)
            {
                Debug.LogWarning($"Ԥ���� {weaponData.id} û��TrapBase���");
                Object.Destroy(weaponObj);
                return null;
            }

            weapon.InitWeaponData(weaponData);
            weaponObj.transform.SetParent(parent);
            weaponObj.transform.transform.localPosition = position;
            weapon.SetOnHandOrNot(true);
            return weaponObj;
        }

    }

}
