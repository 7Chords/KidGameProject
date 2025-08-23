using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 普通泛型单例基类(不继承mono)
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

