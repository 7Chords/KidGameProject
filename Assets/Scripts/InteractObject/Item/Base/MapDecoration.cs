using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 场景中的装饰物，不需要实现具体的逻辑
    /// </summary>
    public class MapDecoration : MonoBehaviour, IInteractive
    {

        public virtual void InteractNegative() 
        {

        }

        public virtual void InteractPositive() { }
    }
}