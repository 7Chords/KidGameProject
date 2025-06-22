using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemySearchTargetRoomState : EnemyStateBase
    {
        // private Room _currentRoom;
        // private Room[] _roomsOrdered;
        
        private int  _roomIndex;
        private string _wantedItemId;
        
        public void SetTarget(string itemId)
        {
            _wantedItemId = itemId;
        }

        public override void Enter()
        {
            // 获取所有房间
            
            // _roomsOrdered = allRooms.OrderBy(r => 
            //     Vector3.Distance(enemy.transform.position, r.transform.position)).ToArray();
            // _roomIndex = 0;
            // PickNextRoom();
            
            // enemy.PlayAnimation("Walk");
        }

        public override void Update()
        {
            // if (_currentRoom == null) { enemy.ChangeState(EnemyState.Idle); return; }
            //
            // Vector3 dir = (_currentRoom.EntryPoint - enemy.transform.position).normalized;
            // enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;
            //
            // if (Vector3.Distance(enemy.transform.position, _currentRoom.EntryPoint) < 0.2f)
            // {
            //     enemy.Rb.velocity = Vector3.zero;
            //     // 检查房间是否有目标物品
            //     if (_currentRoom.HasItem(_wantedItemId))
            //     {
            //         _currentRoom.TakeItem(_wantedItemId);
            //         enemy.ChangeState(EnemyState.Idle);
            //     }
            //     else
            //     {
            //         _roomIndex++;
            //         if (!PickNextRoom()) enemy.ChangeState(EnemyState.Idle);
            //     }
            // }

            // 若途中看到玩家立即追击
            if (enemy.PlayerInSight()) enemy.ChangeState(EnemyState.Chase);
        }

        public override void Exit() => enemy.Rb.velocity = Vector3.zero;

        private bool PickNextRoom()
        {
            // if (_roomIndex >= _roomsOrdered.Length) return false;
            // _currentRoom = _roomsOrdered[_roomIndex];
            return true;
        }
    }
}
