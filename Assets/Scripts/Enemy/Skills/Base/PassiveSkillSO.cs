using UnityEngine;


namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "New Passive Skill", menuName = "KidGameSO/Skills/Passive Skill")]
    public class PassiveSkillSO : ScriptableObject
    {
        public string skillId;
        public string skillName;
        [TextArea] public string skillDescription;
        
        public virtual void Apply(EnemyController enemy)
        {
            // 基础被动效果
            Debug.Log($"应用被动技能: {skillName}");
        }
        
        public virtual void Remove(EnemyController enemy)
        {
            // 移除被动效果
            Debug.Log($"移除被动技能: {skillName}");
        }
    }
}