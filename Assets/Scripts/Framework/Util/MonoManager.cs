using System;
using UnityEngine;

//Ϊ���̳�MonoBehavior�Ľű��ṩupdate fixedupdate�ȸ��º���ִ���¼�
public class MonoManager : Singleton<MonoManager>
{
    private Action _updateEvent;
    private Action _lateUpdateEvent;
    private Action _fixedUpdateEvent;

    /// <summary>
    /// ���Update����
    /// </summary>
    /// <param name="action"></param>
    public void AddUpdateListener(Action action)
    {
        _updateEvent += action;
    }

    /// <summary>
    /// �Ƴ�Update����
    /// </summary>
    /// <param name="action"></param>
    public void RemoveUpdateListener(Action action)
    {
        _updateEvent -= action;
    }

    /// <summary>
    /// ���LateUpdate����
    /// </summary>
    /// <param name="action"></param>
    public void AddLateUpdateListener(Action action)
    {
        _lateUpdateEvent += action;
    }

    /// <summary>
    /// �Ƴ�LateUpdate����
    /// </summary>
    /// <param name="action"></param>
    public void RemoveLateUpdateListener(Action action)
    {
        _lateUpdateEvent -= action;
    }

    /// <summary>
    /// ���FixedUpdate����
    /// </summary>
    /// <param name="action"></param>
    public void AddFixedUpdateListener(Action action)
    {
        _fixedUpdateEvent += action;
    }

    /// <summary>
    /// �Ƴ�FixedUpdate����
    /// </summary>
    /// <param name="action"></param>
    public void RemoveFixedUpdateListener(Action action)
    {
        _fixedUpdateEvent -= action;
    }

    private void Update()
    {
        _updateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        _lateUpdateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        _fixedUpdateEvent?.Invoke();
    }

    public GameObject InstantiateGameObject(GameObject obj)
    {
        GameObject go = Instantiate(obj);
        return go;
    }
}