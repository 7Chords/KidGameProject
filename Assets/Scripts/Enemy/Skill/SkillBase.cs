using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 技能基类
    /// </summary>
    public abstract class SkillBase : ScriptableObject
    {
        [Header("基础属性")] public string skillName;
        public float cooldownTime = 2f; // 冷却时间
        public bool isPassive; // 是否被动技能

        [NonSerialized] public float lastCastTime; // 上次释放时间
        [NonSerialized] public EnemyController enemy; // 行为树黑板引用

        /// <summary>
        /// 检查技能是否可触发
        /// </summary>
        public abstract bool CanTrigger(GameObject caster);

        /// <summary>
        /// 执行技能效果
        /// </summary>
        public abstract void Trigger(GameObject caster, GameObject target = null);

        /// <summary>
        /// 检查是否处于冷却中
        /// </summary>
        protected bool IsInCooldown()
        {
            return Time.time - lastCastTime < cooldownTime;
        }
    }
}