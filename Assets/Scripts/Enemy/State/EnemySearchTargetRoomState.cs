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
//        private const float SearchDuration = 3.0f; // 搜索一个房间的时间

//        public override void Enter()
//        {
//            _hasFoundItem = false;
//            _searchTimer = 0f;
            
//            // 获取所有房间并按距离排序
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
//                // 如果没有房间可搜索，回到巡逻状态
//                enemy.ChangeState(EnemyState.Patrol);
//            }
//        }

//        public override void Update()
//        {
//            // 如果发现玩家，优先追击
//            if (enemy.PlayerInSight() || enemy.PlayerInHearing())
//            {
//                enemy.ChangeState(EnemyState.Chase);
//                return;
//            }

//            // 已经找到物品，返回巡逻状态
//            if (_hasFoundItem)
//            {
//                enemy.ChangeState(EnemyState.Patrol);
//                return;
//            }

//            // 正在前往目标房间
//            if (_currentTargetRoom != null && _searchTimer <= 0)
//            {
//                Vector3 dir = (_currentTargetRoom.CenterWorldPosition - enemy.transform.position).normalized;
//                enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

//                // 到达目标房间
//                if (Vector3.Distance(enemy.transform.position, _currentTargetRoom.CenterWorldPosition) < 1.0f)
//                {
//                    enemy.Rb.velocity = Vector3.zero;
//                    _searchTimer = SearchDuration;
//                    StartSearchInRoom();
//                }
//            }
//            // 正在搜索当前房间
//            else if (_searchTimer > 0)
//            {
//                _searchTimer -= Time.deltaTime;
                
//                // 搜索完成
//                if (_searchTimer <= 0)
//                {
//                    RoomSearchCompleted();
//                }
//            }
//        }

//        private void StartSearchInRoom()
//        {
//            // 根据房间类型播放不同的搜索动画
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
            
//            Debug.Log($"开始在 {_currentTargetRoom.RoomType} 房间搜索物品 {_targetItemId}");
//        }

//        private void RoomSearchCompleted()
//        {
//            // todo:检查是否找到目标物品，根据敌人的视阈判断
//            bool itemFound = true;
            
//            if (itemFound)
//            {
//                _hasFoundItem = true;
//                Debug.Log($"在 {_currentTargetRoom.RoomType} 房间找到了物品 {_targetItemId}");
//            }
//            else
//            {
//                // 前往下一个房间
//                _roomIndex++;
//                if (_roomIndex < _roomsToCheck.Count)
//                {
//                    _currentTargetRoom = _roomsToCheck[_roomIndex];
//                    Debug.Log($"未找到物品，前往下一个房间: {_currentTargetRoom.RoomType}");
//                }
//                else
//                {
//                    // 所有房间都搜索完毕，未找到物品
//                    Debug.Log("所有房间都搜索完毕，未找到物品");
//                    enemy.ChangeState(EnemyState.Patrol);
//                }
//            }
//        }

//        public override void Exit()
//        {
//            enemy.Rb.velocity = Vector3.zero;
//        }

//        // 从外部设置要寻找的物品ID
//        public void SetTargetItem(string itemId)
//        {
//            _targetItemId = itemId;
//        }
//    }
//}