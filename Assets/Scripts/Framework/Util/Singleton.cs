using UnityEngine;

/// <summary>
/// 普通泛型单例基类(mono)
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }
}