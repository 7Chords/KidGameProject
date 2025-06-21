using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ���Ϲ���
    /// </summary>
    public static class MaterialFactory
    {
        public static GameObject Create(string materialID, Vector3 position)
        {
            //����Ӧ�ø���trapID���ض�Ӧ��Ԥ����
            //ʵ����Ŀ��Ӧ��ʹ����Դ����ϵͳ

            GameObject materialPrefab = Resources.Load<GameObject>("Materials/" + materialID);
            if (materialPrefab == null)
            {
                Debug.LogWarning($"�Ҳ�������Ԥ����: {materialID}");
                return null;
            }

            GameObject materialObj = Object.Instantiate(materialPrefab, position, Quaternion.identity);
            MaterialBase material = materialObj.GetComponent<MaterialBase>();

            if (material == null)
            {
                Debug.LogWarning($"Ԥ���� {materialID} û��TrapBase���");
                Object.Destroy(materialObj);
                return null;
            }

            return materialObj;
        }
    }
}
