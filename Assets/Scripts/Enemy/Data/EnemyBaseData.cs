using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "New EnemyBaseData", menuName = "KidGameSO/Enemy/EnemyBaseData")]
    public class EnemyBaseData : ScriptableObject
    {
        public string EnemyId;
        public string EnemyName;
        public float MaxSanity = 100f;

        public float MoveSpeed = 2.0f;
        public float Resistance = 0.5f;

        public float VisionRange = 8f;
        public float HearingRange = 4f;
        
        public List<ActiveSkillSO> activeSkills = new List<ActiveSkillSO>();
        public List<PassiveSkillSO> passiveSkills = new List<PassiveSkillSO>();
    }
}