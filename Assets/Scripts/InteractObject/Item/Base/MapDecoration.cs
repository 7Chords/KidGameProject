using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// �����е�װ�������Ҫʵ�־�����߼�
    /// </summary>
    public class MapDecoration : MonoBehaviour, IInteractive
    {

        public virtual void InteractNegative() 
        {

        }

        public virtual void InteractPositive() { }
    }
}