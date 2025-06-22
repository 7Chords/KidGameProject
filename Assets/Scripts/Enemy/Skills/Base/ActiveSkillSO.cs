using UnityEngine;

namespace KidGame.Core
{
    public enum SkillTriggerCondition
    {
        OnSpawn,
        OnAttack,
        OnHit,
        OnLowHealth,
        OnTimer,
        OnPlayerInRange
    }

    [CreateAssetMenu(fileName = "New Active Skill", menuName = "KidGameSO/Skills/Active Skill")]
    public class ActiveSkillSO : ScriptableObject
    {
        public string skillId;
        public string skillName;
        [TextArea] public string skillDescription;
        public SkillTriggerCondition triggerCondition;
        public float cooldown;
        
        public float triggerRange;
        [Range(0, 1)] public float healthThreshold = 0.3f;
        public float timerInterval;
        
        public virtual void Execute(EnemyController enemy)
        {
            Debug.Log($"Ö´ÐÐ¼¼ÄÜ: {skillName}");
        }
    }
}