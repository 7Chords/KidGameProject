using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace KidGame.Core
{
    public static class KidGameExtension
    {
        #region ͨ��

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public static T GetAttribute<T>(this object obj) where T : Attribute
        {
            return obj.GetType().GetCustomAttribute<T>();
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="type">�������ڵ�����</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj, Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }

        /// <summary>
        /// ������ȶԱ�
        /// </summary>
        public static bool ArraryEquals(this object[] objs, object[] other)
        {
            if (other == null || objs.GetType() != other.GetType())
            {
                return false;
            }

            if (objs.Length == other.Length)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!objs[i].Equals(other[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion

        #region ��Դ����

        /// <summary>
        /// GameObject��������
        /// </summary>
        public static void GameObjectPushPool(this GameObject go)
        {
            PoolManager.Instance.PushGameObject(go);
        }

        /// <summary>
        /// GameObject��������
        /// </summary>
        public static void GameObjectPushPool(this Component com)
        {
            GameObjectPushPool(com.gameObject);
        }

        /// <summary>
        /// ��ͨ��Ž�����
        /// </summary>
        /// <param name="obj"></param>
        public static void ObjectPushPool(this object obj)
        {
            PoolManager.Instance.PushObject(obj);
        }

        #endregion

        #region Mono

        /// <summary>
        /// ���Update����
        /// </summary>
        public static void OnUpdate(this object obj, Action action)
        {
            MonoManager.Instance.AddUpdateListener(action);
        }

        /// <summary>
        /// �Ƴ�Update����
        /// </summary>
        public static void RemoveUpdate(this object obj, Action action)
        {
            MonoManager.Instance.RemoveUpdateListener(action);
        }

        /// <summary>
        /// ���LateUpdate����
        /// </summary>
        public static void OnLateUpdate(this object obj, Action action)
        {
            MonoManager.Instance.AddLateUpdateListener(action);
        }

        /// <summary>
        /// �Ƴ�LateUpdate����
        /// </summary>
        public static void RemoveLateUpdate(this object obj, Action action)
        {
            MonoManager.Instance.RemoveLateUpdateListener(action);
        }

        /// <summary>
        /// ���FixedUpdate����
        /// </summary>
        public static void OnFixedUpdate(this object obj, Action action)
        {
            MonoManager.Instance.AddFixedUpdateListener(action);
        }

        /// <summary>
        /// �Ƴ�Update����
        /// </summary>
        public static void RemoveFixedUpdate(this object obj, Action action)
        {
            MonoManager.Instance.RemoveFixedUpdateListener(action);
        }

        public static Coroutine StartCoroutine(this object obj, IEnumerator routine)
        {
            return MonoManager.Instance.StartCoroutine(routine);
        }

        public static void StopCoroutine(this object obj, Coroutine routine)
        {
            MonoManager.Instance.StopCoroutine(routine);
        }

        public static void StopAllCoroutines(this object obj)
        {
            MonoManager.Instance.StopAllCoroutines();
        }

        #endregion

        #region GameObject

        public static bool IsNull(this GameObject obj)
        {
            return ReferenceEquals(obj, null);
        }

        #endregion
    }
}