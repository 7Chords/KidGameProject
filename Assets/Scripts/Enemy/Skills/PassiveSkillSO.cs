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
            // ��������Ч��
            Debug.Log($"Ӧ�ñ�������: {skillName}");
        }
        
        public virtual void Remove(EnemyController enemy)
        {
            // �Ƴ�����Ч��
            Debug.Log($"�Ƴ���������: {skillName}");
        }
    }
}