using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// 敌人条件节点基类
    /// </summary>
    public class BaseEnemyConditional : Conditional
    {
        protected EnemyController enemy;
        protected NavMeshAgent agent;
        protected Animator animator;

        public override void OnAwake()
        {
            enemy = gameObject.GetComponent<EnemyController>();
            agent = enemy.Agent;
            animator = enemy.Animator;
        }

    }
}
