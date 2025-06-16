using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  �����еĿ�ʰȡ��Ʒ
    /// </summary>
    public abstract class MapItem : MonoBehaviour, IPickable,IInteractive
    {
        /// <summary>
        /// ��������
        /// </summary>
        public abstract void InteractPositive();

        /// <summary>
        /// ��������
        /// </summary>
        public abstract void InteractNegative();

        /// <summary>
        /// ʰȡ
        /// </summary>
        public abstract void Pick();
    }
}