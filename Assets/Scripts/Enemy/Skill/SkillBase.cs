using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.AI;

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
        
        public abstract void Init(EnemyController enemyController);
        
        /// <summary>
        /// 检查技能是否可触发
        /// </summary>
        public abstract bool CanTrigger();
        
        //具体技能使用逻辑返回值可能各异，并且也会在Node中有具体使用的逻辑，所以不写抽象方法
        
        
        /// <summary>
        /// 检查是否处于冷却中
        /// </summary>
        public bool IsInCooldown()
        {
            return Time.time - lastCastTime < cooldownTime;
        }
    }
}