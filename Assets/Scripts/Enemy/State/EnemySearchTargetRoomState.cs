using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemySearchTargetRoomState : EnemyStateBase
    {
        private List<RoomInfo> _roomsToCheck = new(); // 自定义结构体或类
        private RoomInfo _currentTargetRoom;
        
        private int _roomIndex;
        private string _wantedItemId;

        public void SetTarget(string itemId)
        {
            _wantedItemId = itemId;
        }

        public override void Enter()
        {
            // 构造房间列表
            _roomsToCheck = MapManager.Instance.GetAllRooms()
                .OrderBy(r => Vector3.Distance(enemy.transform.position, r.CenterWorldPosition))
                .ToList();

            _roomIndex = 0;
            GoToNextRoom();
        }

        private void GoToNextRoom()
        {
            if (_roomIndex >= _roomsToCheck.Count)
            {
                enemy.ChangeState(EnemyState.Patrol); // 没找到
                return;
            }

            _currentTargetRoom = _roomsToCheck[_roomIndex];
            _roomIndex++;
        }

        public override void Update()
        {
            if (_currentTargetRoom == null) return;

            // 前往当前房间
            Vector3 dir = (_currentTargetRoom.CenterWorldPosition - enemy.transform.position).normalized;
            enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

            if (Vector3.Distance(enemy.transform.position, _currentTargetRoom.CenterWorldPosition) < 0.2f)
            {
                enemy.Rb.velocity = Vector3.zero;

                // 找目标物品，没找到就去下一个房间
                if (true)
                {
                    enemy.ChangeState(EnemyState.Patrol); // 找到目标
                }
                else
                {
                    GoToNextRoom(); // 没找到，下一个房间
                }
            }

            // 若中途看到玩家，立即追击
            if (enemy.PlayerInSight()) enemy.ChangeState(EnemyState.Chase);
        }
    }
}