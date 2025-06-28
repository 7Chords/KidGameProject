//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace KidGame.Core
//{
//    public class EnemySearchTargetRoomState : EnemyStateBase
//    {
//        private List<RoomInfo> _roomsToCheck = new List<RoomInfo>();
//        private RoomInfo _currentTargetRoom;
//        private int _roomIndex;
//        private string _targetItemId;
//        private bool _hasFoundItem;
//        private float _searchTimer;
//        private const float SearchDuration = 3.0f; // ����һ�������ʱ��

//        public override void Enter()
//        {
//            _hasFoundItem = false;
//            _searchTimer = 0f;
            
//            // ��ȡ���з��䲢����������
//            _roomsToCheck = MapManager.Instance.GetAllRooms()
//                .OrderBy(r => Vector3.Distance(enemy.transform.position, r.CenterWorldPosition))
//                .ToList();
            
//            _roomIndex = 0;
//            if (_roomsToCheck.Count > 0)
//            {
//                _currentTargetRoom = _roomsToCheck[0];
//                // enemy.PlayAnimation("Walk");
//            }
//            else
//            {
//                // ���û�з�����������ص�Ѳ��״̬
//                enemy.ChangeState(EnemyState.Patrol);
//            }
//        }

//        public override void Update()
//        {
//            // ���������ң�����׷��
//            if (enemy.PlayerInSight() || enemy.PlayerInHearing())
//            {
//                enemy.ChangeState(EnemyState.Chase);
//                return;
//            }

//            // �Ѿ��ҵ���Ʒ������Ѳ��״̬
//            if (_hasFoundItem)
//            {
//                enemy.ChangeState(EnemyState.Patrol);
//                return;
//            }

//            // ����ǰ��Ŀ�귿��
//            if (_currentTargetRoom != null && _searchTimer <= 0)
//            {
//                Vector3 dir = (_currentTargetRoom.CenterWorldPosition - enemy.transform.position).normalized;
//                enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

//                // ����Ŀ�귿��
//                if (Vector3.Distance(enemy.transform.position, _currentTargetRoom.CenterWorldPosition) < 1.0f)
//                {
//                    enemy.Rb.velocity = Vector3.zero;
//                    _searchTimer = SearchDuration;
//                    StartSearchInRoom();
//                }
//            }
//            // ����������ǰ����
//            else if (_searchTimer > 0)
//            {
//                _searchTimer -= Time.deltaTime;
                
//                // �������
//                if (_searchTimer <= 0)
//                {
//                    RoomSearchCompleted();
//                }
//            }
//        }

//        private void StartSearchInRoom()
//        {
//            // ���ݷ������Ͳ��Ų�ͬ����������
//            switch (_currentTargetRoom.RoomType)
//            {
//                case RoomType.Bedroom:
//                    // enemy.PlayAnimation("SearchBed");
//                    break;
//                case RoomType.LivingRoom:
//                    // enemy.PlayAnimation("SearchSofa");
//                    break;
//                default:
//                    // enemy.PlayAnimation("SearchGeneric");
//                    break;
//            }
            
//            Debug.Log($"��ʼ�� {_currentTargetRoom.RoomType} ����������Ʒ {_targetItemId}");
//        }

//        private void RoomSearchCompleted()
//        {
//            // todo:����Ƿ��ҵ�Ŀ����Ʒ�����ݵ��˵������ж�
//            bool itemFound = true;
            
//            if (itemFound)
//            {
//                _hasFoundItem = true;
//                Debug.Log($"�� {_currentTargetRoom.RoomType} �����ҵ�����Ʒ {_targetItemId}");
//            }
//            else
//            {
//                // ǰ����һ������
//                _roomIndex++;
//                if (_roomIndex < _roomsToCheck.Count)
//                {
//                    _currentTargetRoom = _roomsToCheck[_roomIndex];
//                    Debug.Log($"δ�ҵ���Ʒ��ǰ����һ������: {_currentTargetRoom.RoomType}");
//                }
//                else
//                {
//                    // ���з��䶼������ϣ�δ�ҵ���Ʒ
//                    Debug.Log("���з��䶼������ϣ�δ�ҵ���Ʒ");
//                    enemy.ChangeState(EnemyState.Patrol);
//                }
//            }
//        }

//        public override void Exit()
//        {
//            enemy.Rb.velocity = Vector3.zero;
//        }

//        // ���ⲿ����ҪѰ�ҵ���ƷID
//        public void SetTargetItem(string itemId)
//        {
//            _targetItemId = itemId;
//        }
//    }
//}