// NeuroticPassiveSkill.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "NeuroticPassive", menuName = "KidGameSO/Enemy/NeuroticPassive")]
    public class NeuroticPassiveSkill : ScriptableObject
    {
        [Header("触发参数")]
        [Tooltip("触发间隔（秒），建议5-20秒")]
        public float triggerInterval = 10f;
        
        [Tooltip("停止持续时间（秒），建议2-5秒")]
        public float stopDuration = 3f;

        [Header("表现参数")]
        public bool playStopAnimation = true;
        public string stopAnimTrigger = "Stunned";

        [System.NonSerialized] public EnemyController enemy;
        [System.NonSerialized] public NavMeshAgent navAgent;
        [System.NonSerialized] public bool isStopping;
        [System.NonSerialized] private float lastTriggerTime;

        // 初始化（绑定敌人组件）
        public void Init(EnemyController enemyController)
        {
            enemy = enemyController;
            navAgent = enemy.GetComponent<NavMeshAgent>();
            lastTriggerTime = Time.time; // 不允许入局触发
        }

        // 检查是否达到触发条件（内部计时器）
        public bool CheckTriggerCondition()
        {
            if (isStopping) return false; // 防止重复触发
            return Time.time - lastTriggerTime >= triggerInterval;
        }

        // 开始停止流程
        public IEnumerator StartStopCoroutine()
        {
            Debug.Log("神经质大叫");
            isStopping = true;
            lastTriggerTime = Time.time;
            
            // 1. 记录当前状态（用于恢复）
            bool wasNavEnabled = navAgent.enabled;
            Vector3 originalVelocity = enemy.Rb.velocity;

            // 2. 停止移动
            enemy.Rb.velocity = Vector3.zero;
            if (navAgent != null) navAgent.enabled = false;

            // 3. 播放动画（如果需要）
            if (playStopAnimation && enemy.Animator != null)
            {
                enemy.Animator.SetTrigger(stopAnimTrigger);
            }

            // 4. 等待停止时长
            yield return new WaitForSeconds(stopDuration);

            Debug.Log("大叫结束");
            // 5. 恢复状态
            EndStop(wasNavEnabled, originalVelocity);
        }

        // 结束停止状态
        public void EndStop(bool restoreNavState, Vector3 originalVelocity)
        {
            if (navAgent != null) navAgent.enabled = restoreNavState;
            enemy.Rb.velocity = originalVelocity;
            isStopping = false;
        }

        // 强制终止停止（如被打断时）
        public void ForceEndStop()
        {
            if (isStopping)
            {
                isStopping = false;
                if (navAgent != null) navAgent.enabled = true;
                enemy.Rb.velocity = Vector3.zero;
            }
        }
    }
}
