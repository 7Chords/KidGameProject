using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{ 

    /// <summary>
    /// �����˵Ľӿ�
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// ���˷���
        /// </summary>
        /// <param name="damageInfo">�˺���Ϣ</param>
        public abstract void TakeDamage(DamageInfo damageInfo);

    }
}
