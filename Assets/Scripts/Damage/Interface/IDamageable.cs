using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{ 

    /// <summary>
    /// 可受伤的接口
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 受伤方法
        /// </summary>
        /// <param name="damageInfo">伤害信息</param>
        public abstract void TakeDamage(DamageInfo damageInfo);

        //受伤随机音效列表
        public abstract List<string>  RandomDamgeSfxList { get; set; }

        //受伤粒子特效
        public abstract ParticleSystem DamagePartical { get; set; }
    }
}
