using System.Collections;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "Dash Skill", menuName = "KidGameSO/Skills/Dash Skill")]
    public class DashSkillSO : ActiveSkillSO
    {
        public float dashSpeedMultiplier = 3f;
        public float dashDuration = 0.5f;

        public override void Execute(EnemyController enemy)
        {
            base.Execute(enemy);
            enemy.StartCoroutine(DashRoutine(enemy));
        }

        private IEnumerator DashRoutine(EnemyController enemy)
        {
            float originalSpeed = enemy.EnemyBaseData.MoveSpeed;
            float dashSpeed = originalSpeed * dashSpeedMultiplier;
            float startTime = Time.time;

            while (Time.time < startTime + dashDuration)
            {
                enemy.Rb.velocity = enemy.transform.forward * dashSpeed;
                yield return null;
            }
        }
    }
}