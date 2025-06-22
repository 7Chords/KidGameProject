using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemySearchTargetRoomState : EnemyStateBase
    {
        private List<RoomInfo> _roomsToCheck = new(); // �Զ���ṹ�����
        private RoomInfo _currentTargetRoom;
        
        private int _roomIndex;
        private string _wantedItemId;

        public void SetTarget(string itemId)
        {
            _wantedItemId = itemId;
        }

        public override void Enter()
        {
            // ���췿���б�
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
                enemy.ChangeState(EnemyState.Patrol); // û�ҵ�
                return;
            }

            _currentTargetRoom = _roomsToCheck[_roomIndex];
            _roomIndex++;
        }

        public override void Update()
        {
            if (_currentTargetRoom == null) return;

            // ǰ����ǰ����
            Vector3 dir = (_currentTargetRoom.CenterWorldPosition - enemy.transform.position).normalized;
            enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

            if (Vector3.Distance(enemy.transform.position, _currentTargetRoom.CenterWorldPosition) < 0.2f)
            {
                enemy.Rb.velocity = Vector3.zero;

                // ��Ŀ����Ʒ��û�ҵ���ȥ��һ������
                if (true)
                {
                    enemy.ChangeState(EnemyState.Patrol); // �ҵ�Ŀ��
                }
                else
                {
                    GoToNextRoom(); // û�ҵ�����һ������
                }
            }

            // ����;������ң�����׷��
            if (enemy.PlayerInSight()) enemy.ChangeState(EnemyState.Chase);
        }
    }
}