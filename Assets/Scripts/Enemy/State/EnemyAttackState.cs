//using KidGame.Core;
//using UnityEngine;

//namespace KidGame.Core
//{
//    /// <summary>
//    /// ¹¥»÷£¬³ÖÐø³¯Íæ¼Ò×·»÷¡£³¬³öÊÓÌý·¶Î§Ôò·µ»ØÑ²Âß
//    /// </summary>
//    public class EnemyAttackState : EnemyStateBase
//    {
//        private const float _attackDistance = 1.5f;
//        private Transform _player;

//        public override void Enter()
//        {
//            _player = enemy.Player;
//            // enemy.PlayAnimation("Run");
//        }

//        public override void Update()
//        {
//            if (_player == null)
//            {
//                enemy.ChangeState(EnemyState.Idle);
//                return;
//            }

//            Vector3 dir = (_player.position - enemy.transform.position).normalized;
//            enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

//            float dist = Vector3.Distance(enemy.transform.position, _player.position);
//            if (dist <= _attackDistance)
//            {
//                enemy.Rb.velocity = Vector3.zero;
//                // enemy.PlayAnimation("Attack");
//            }
//            else if (!enemy.PlayerInSight() && !enemy.PlayerInHearing())
//            {
//                enemy.ChangeState(EnemyState.Patrol);
//            }
//        }

//        public override void Exit() => enemy.Rb.velocity = Vector3.zero;
//    }
//}