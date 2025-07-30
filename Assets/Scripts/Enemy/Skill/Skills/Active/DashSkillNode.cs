using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class DashSkillNode : BaseEnemyAction {
        [SerializeField] private DashSkill dashSkill; // ???????????
        private bool isDashing = false; // §¿???????
        private Coroutine dashCoroutine; // §¿??????

        public override void OnAwake()
        {
            base.OnAwake();
            dashSkill.Init(enemy);
        }


        public override TaskStatus OnUpdate() {
            // 1. §¿??????? ?? ????Running???????
            if (isDashing)
            {
                return TaskStatus.Running;
            }

            // 2. ??ýG?????????
            if (!dashSkill.CanTrigger())
            {
                return TaskStatus.Failure;
            }

            // ????????????????§¿???????????
            dashCoroutine = enemy.StartCoroutine(PrepareAndDashCoroutine());
            return TaskStatus.Running;
        }
        
        // ?????????????????????§¿??
        private IEnumerator PrepareAndDashCoroutine()
        {
            isDashing = true;

            // 1. ????????¦Ë?????????????
            Vector3 recordedPlayerPos = enemy.Player.position;
            enemy.Rb.velocity = Vector3.zero;
            dashSkill.StopNavAgent();
            // 2. ?????????????¦Ë??
            yield return StartCoroutine(TurnToTarget(recordedPlayerPos));

            // 3. ???????????????????
            yield return new WaitForSeconds(dashSkill.prepareTime);

            // 4. ??§Ô????????????¦Ë???
            yield return enemy.StartCoroutine(dashSkill.TriggerCoroutine(recordedPlayerPos));

            // 5. ??????
            isDashing = false;
            dashCoroutine = null;
        }
        // ????????????§¿??
        private IEnumerator TurnToTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - enemy.transform.position).normalized;
            lookDirection.y = 0; // ????Y????????????

            if (lookDirection.sqrMagnitude < 0.01f) yield break; // ?????????????

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            float rotateSpeed = dashSkill.rotateSpeed;

            // ?????????
            while (Quaternion.Angle(enemy.transform.rotation, targetRotation) > 10f)
            {
                enemy.transform.rotation = Quaternion.Lerp(
                    enemy.transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }
            enemy.transform.rotation = targetRotation; // ????????????¦Ë
        }
        

        // ????§Ø?????§¿????Àà????????????????
        public override void OnEnd()
        {
            if (dashCoroutine != null)
            {
                enemy.StopCoroutine(dashCoroutine);
                isDashing = false;
                dashCoroutine = null;
                dashSkill.StopDash(); // ????????????
            }
        }
    }
}