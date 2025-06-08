using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    // ���ڵ�
    [SerializeField]
    private GameObject poolRootObj;
    /// <summary>
    /// GameObject��������
    /// </summary>
    public Dictionary<string, GameObjectPoolData> gameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();
    /// <summary>
    /// ��ͨ�� ��������
    /// </summary>
    public Dictionary<string, ObjectPoolData> objectPoolDic = new Dictionary<string, ObjectPoolData>();


    public int initSpawnAmount = 3;

    #region GameObject������ز���

    /// <summary>
    /// ��ȡGameObject,�������û���򷵻�Null
    /// </summary>
    public GameObject GetGameObject(string assetName, GameObject prefab = null, Transform parent = null)
    {
        GameObject obj = null;
        if (!gameObjectPoolDic.ContainsKey(assetName))
        {
            for (int i = 0; i < initSpawnAmount; i++)
            {
                obj = Instantiate(prefab);
                PushGameObject(obj);
            }
            return GetGameObject(assetName);
        }
        // �����û����һ��
        if (gameObjectPoolDic.TryGetValue(assetName, out GameObjectPoolData poolData) && poolData.poolQueue.Count > 0)
        {
            obj = poolData.GetObj(parent);
        }
        return obj;
    }

    /// <summary>
    /// GameObject�Ž������
    /// </summary>
    public void PushGameObject(GameObject obj)
    {
        string name = obj.name.Replace("(Clone)", "");
        // ������û����һ��
        if (gameObjectPoolDic.TryGetValue(name, out GameObjectPoolData poolData))
        {
            poolData.PushObj(obj);
        }
        else
        {
            gameObjectPoolDic.Add(name, new GameObjectPoolData(obj, poolRootObj));
        }
    }

    #endregion

    #region ��ͨ������ز���
    /// <summary>
    /// ��ȡ��ͨ����
    /// </summary>
    public T GetObject<T>() where T : class, new()
    {
        T obj;
        if (CheckObjectCache<T>())
        {
            string name = typeof(T).FullName;
            obj = (T)objectPoolDic[name].GetObj();
            return obj;
        }
        else
        {
            for (int i = 0; i < initSpawnAmount; i++)
            {
                obj = new T();
                PushObject(obj);
            }
            return GetObject<T>();
        }
    }

    /// <summary>
    /// GameObject�Ž������
    /// </summary>
    /// <param name="obj"></param>
    public void PushObject(object obj)
    {
        string name = obj.GetType().FullName;
        // ������û����һ��
        if (objectPoolDic.ContainsKey(name))
        {
            objectPoolDic[name].PushObj(obj);
        }
        else
        {
            objectPoolDic.Add(name, new ObjectPoolData(obj));
        }
    }

    private bool CheckObjectCache<T>()
    {
        string name = typeof(T).FullName;
        return objectPoolDic.ContainsKey(name) && objectPoolDic[name].poolQueue.Count > 0;
    }
    #endregion


    #region ɾ��
    /// <summary>
    /// ɾ��ȫ��
    /// </summary>
    /// <param name="clearGameObject">�Ƿ�ɾ����Ϸ����</param>
    /// <param name="clearCObject">�Ƿ�ɾ����ͨC#����</param>
    public void Clear(bool clearGameObject = true, bool clearCObject = true)
    {
        if (clearGameObject)
        {
            for (int i = 0; i < poolRootObj.transform.childCount; i++)
            {
                Destroy(poolRootObj.transform.GetChild(i).gameObject);
            }
            gameObjectPoolDic.Clear();
        }

        if (clearCObject)
        {
            objectPoolDic.Clear();
        }
    }

    public void ClearAllGameObject()
    {
        Clear(true, false);
    }
    public void ClearGameObject(string prefabName)
    {
        GameObject go = poolRootObj.transform.Find(prefabName).gameObject;
        if (go.IsUnityNull() == false)
        {
            Destroy(go);
            gameObjectPoolDic.Remove(prefabName);

        }

    }
    public void ClearGameObject(GameObject prefab)
    {
        ClearGameObject(prefab.name);
    }

    public void ClearAllObject()
    {
        Clear(false, true);
    }
    public void ClearObject<T>()
    {
        objectPoolDic.Remove(typeof(T).FullName);
    }
    public void ClearObject(Type type)
    {
        objectPoolDic.Remove(type.FullName);
    }
    #endregion

}
