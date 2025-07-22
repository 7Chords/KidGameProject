using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// ���ܻ���
    /// </summary>
    public abstract class SkillBase : ScriptableObject
    {
        [Header("��������")] public string skillName;
        public float cooldownTime = 2f; // ��ȴʱ��
        public bool isPassive; // �Ƿ񱻶�����

        [NonSerialized] public float lastCastTime; // �ϴ��ͷ�ʱ��
        
        public abstract void Init(EnemyController enemyController);
        
        /// <summary>
        /// ��鼼���Ƿ�ɴ���
        /// </summary>
        public abstract bool CanTrigger();
        
        //���弼��ʹ���߼�����ֵ���ܸ��죬����Ҳ����Node���о���ʹ�õ��߼������Բ�д���󷽷�
        public virtual void Trigger(){}
        
        
        /// <summary>
        /// ����Ƿ�����ȴ��
        /// </summary>
        public bool IsInCooldown()
        {
            return Time.time - lastCastTime < cooldownTime;
        }
    }
}