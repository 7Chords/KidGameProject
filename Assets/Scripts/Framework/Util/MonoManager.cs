using System;
using UnityEngine;

//为不继承MonoBehavior的脚本提供update fixedupdate等更新函数执行事件
public class MonoManager : Singleton<MonoManager>
{
    private Action _updateEvent;
    private Action _lateUpdateEvent;
    private Action _fixedUpdateEvent;

    /// <summary>
    /// 添加Update监听
    /// </summary>
    /// <param name="action"></param>
    public void AddUpdateListener(Action action)
    {
        _updateEvent += action;
    }

    /// <summary>
    /// 移除Update监听
    /// </summary>
    /// <param name="action"></param>
    public void RemoveUpdateListener(Action action)
    {
        _updateEvent -= action;
    }

    /// <summary>
    /// 添加LateUpdate监听
    /// </summary>
    /// <param name="action"></param>
    public void AddLateUpdateListener(Action action)
    {
        _lateUpdateEvent += action;
    }

    /// <summary>
    /// 移除LateUpdate监听
    /// </summary>
    /// <param name="action"></param>
    public void RemoveLateUpdateListener(Action action)
    {
        _lateUpdateEvent -= action;
    }

    /// <summary>
    /// 添加FixedUpdate监听
    /// </summary>
    /// <param name="action"></param>
    public void AddFixedUpdateListener(Action action)
    {
        _fixedUpdateEvent += action;
    }

    /// <summary>
    /// 移除FixedUpdate监听
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