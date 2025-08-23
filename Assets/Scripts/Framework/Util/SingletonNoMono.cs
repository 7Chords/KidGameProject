using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��ͨ���͵�������(���̳�mono)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonNoMono<T> where T:class,new()
    {
        protected static T instance = new T();

        public static T Instance
        {
            get 
            {
                if (instance == null)
                    instance = new T();
                return instance; 
            }
        }
    }
}

