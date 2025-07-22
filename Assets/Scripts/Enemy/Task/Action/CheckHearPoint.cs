using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KidGame.Core
{
    /// <summary>
    /// 检查听到声音的位置点
    /// </summary>
    public class CheckHearPoint : BaseEnemyAction
    {
        [Header("到达检测参数")]
        [SerializeField] private float arriveThreshold = 0.5f; // 到达判定阈值
        private bool isMoving = false;
        private NavMeshAgent navAgent;
        private TaskStatus currentStatus = TaskStatus.Running; // 状态标志

        public override void OnStart()
        {
            base.OnStart();
            currentStatus = TaskStatus.Running; // 重置状态
            
            // 获取导航组件
            navAgent = enemy.GetComponent<NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogError("敌人缺少NavMeshAgent组件！");
                currentStatus = TaskStatus.Failure;
                return;
            }
            
            // 启动寻路
            enemy.GoCheckHearPoint();
            isMoving = true;
            
            // 启动到达检测协程
            StartCoroutine(CheckArriveCoroutine());
        }

        public override TaskStatus OnUpdate()
        {
            // 直接返回当前状态（由协程更新）
            return currentStatus;
        }

        // 核心协程：检测到达目标点
        private IEnumerator CheckArriveCoroutine()
        {
            yield return null; // 等待一帧确保路径开始计算

            while (isMoving && navAgent.enabled)
            {
                // 1. 路径计算中，跳过检测
                if (navAgent.pathPending)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                // 2. 检测路径有效性
                if (navAgent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogWarning("导航路径无效或不可达！");
                    currentStatus = TaskStatus.Failure; // 设置失败状态
                    isMoving = false;
                    yield break;
                }

                // 3. 检测是否到达目标
                if (navAgent.remainingDistance <= arriveThreshold)
                {
                    currentStatus = TaskStatus.Success; // 设置成功状态
                    isMoving = false;
                    yield break;
                }

                yield return new WaitForEndOfFrame(); // 每帧检测一次
            }

            // 4. 异常退出时返回失败
            if (isMoving)
            {
                currentStatus = TaskStatus.Failure;
                isMoving = false;
            }
        }

        // 任务中断时清理
        public override void OnEnd()
        {
            isMoving = false;
            currentStatus = TaskStatus.Running; // 重置状态
            StopAllCoroutines();
        }
    }
}
