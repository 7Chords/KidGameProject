using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemyCheckNoiseState : EnemyStateBase
    {
        private Vector3 _soundPos;
        private bool _searched;
        private RoomType _roomType;
        

        public override void Enter()
        {
            _searched = false;
            // enemy.PlayAnimation("Walk");
        }

        public override void Update()
        {
            if (!_searched)
            {
                Vector3 dir = (_soundPos - enemy.transform.position).normalized;
                enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

                if (Vector3.Distance(enemy.transform.position, _soundPos) < 0.3f)
                {
                    enemy.Rb.velocity = Vector3.zero;
                    _searched = true;
                    enemy.StartCoroutine(SearchRoutine());
                }
            }
        }

        private IEnumerator SearchRoutine()
        {
            // 根据房间类型进行不同的搜索逻辑
            switch (_roomType)
            {
                case RoomType.Bedroom:
                    // 假设床在房间的一角，进行床下或柜子搜索
                    yield return new WaitForSeconds(1.0f);
                    CheckUnderBedOrCabinet();
                    break;
                case RoomType.LivingRoom:
                    // 检查沙发下、电视柜等
                    yield return new WaitForSeconds(1.0f);
                    CheckBehindFurniture();
                    break;
                default:
                    yield return new WaitForSeconds(1.0f);
                    break;
            }

            if (enemy.PlayerInSight())  // 找到玩家，进入追击状态
            {
                enemy.Stun(0.5f);
                yield return new WaitForSeconds(0.5f);
                enemy.ChangeState(EnemyState.Chase);
            }
            else
            {
                enemy.ChangeState(EnemyState.Patrol);  // 没有找到玩家，回到巡逻状态
            }
        }

        #region 检查房间

        // 根据房间类型不同，执行不同的搜索行为
        private void CheckUnderBedOrCabinet()
        {
            // 模拟床下和柜子的搜索逻辑
            Debug.Log("正在检查床下或柜子...");
            // 假设在卧室内，敌人会优先检查床下或柜子
        }

        private void CheckBehindFurniture()
        {
            // 模拟检查沙发或电视柜的逻辑
            Debug.Log("正在检查沙发后面...");
            // 若在客厅内，敌人会检查沙发、电视柜后等地方
        }        

        #endregion
        
        public override void Exit()
        {
            enemy.Rb.velocity = Vector3.zero;
        }
    }
}
