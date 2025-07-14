using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

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
        [NonSerialized] public EnemyController enemy; // ��Ϊ���ڰ�����

        /// <summary>
        /// ��鼼���Ƿ�ɴ���
        /// </summary>
        public abstract bool CanTrigger(GameObject caster);

        /// <summary>
        /// ִ�м���Ч��
        /// </summary>
        public abstract void Trigger(GameObject caster, GameObject target = null);

        /// <summary>
        /// ����Ƿ�����ȴ��
        /// </summary>
        protected bool IsInCooldown()
        {
            return Time.time - lastCastTime < cooldownTime;
        }
    }
}