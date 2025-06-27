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

        //���������Ч�б�
        public abstract List<string>  RandomDamgeSfxList { get; set; }

        //����������Ч
        public abstract ParticleSystem DamagePartical { get; set; }
    }
}
