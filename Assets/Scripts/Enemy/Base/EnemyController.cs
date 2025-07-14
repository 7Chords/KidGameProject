using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KidGame.Interface;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using System.Linq;

namespace KidGame.Core
{
    /// <summary>
    /// 封装敌人相关变量与方法
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour, IStateMachineOwner, IDamageable,ISoundable
    {
        [SerializeField]
        private List<string> randomDamgeSfxList;
        public List<string> RandomDamgeSfxList { get => randomDamgeSfxList; set { randomDamgeSfxList = value; } }

        [SerializeField]
        private ParticleSystem damagePartical;
        public ParticleSystem DamagePartical { get => damagePartical; set { damagePartical = value; } }
        
        #region Comp

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Animator animator;
        public Animator Animator => animator;

        private NavMeshAgent agent;
        public NavMeshAgent Agent => agent;

        private BuffHandler enemyBuffHandler;

        private BehaviorTree behaviorTree;

        #endregion

        [SerializeField] private EnemyBaseData enemyBaseData;
        public EnemyBaseData EnemyBaseData => enemyBaseData;

        private Transform player;

        private Dictionary<RoomType, bool> roomSearchStateDic;//房间搜索情况字典

        private List<RoomInfo> _roomsToCheck = new List<RoomInfo>();

        private Vector3 targetPos;

        public Renderer VisionIndicatorRenderer;
        public Color NoSeePlayerColor;
        public Color SeePlayerColor;

        private float curSanity;
        public bool IsDizzying;

        private GameObject hearingGO;

        #region 有目的搜索

        public string _currentTargetItemId;
        
        #endregion
        
        #region 理智变量

        private int _currentHealth;

        #endregion

        #region 所在房间

        private RoomType _currentRoomType;
        public RoomType CurrentRoomType => _currentRoomType;


        #endregion
        
        
        #region 生命周期

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            behaviorTree = GetComponent<BehaviorTree>();
        }


        private void Update()
        {
            UpdateCurrentRoomType();
        }

        public void Init(EnemyBaseData enemyData)
        {
            enemyBaseData = enemyData;
            curSanity = enemyBaseData.MaxSanity;
            //stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            //stateMachine.Init(this);
            //ChangeState(EnemyState.Idle); // 初始状态
            player = FindObjectOfType<PlayerController>().gameObject.transform;

            enemyBuffHandler = new BuffHandler();
            enemyBuffHandler.Init();

            List<RoomInfo> allRoomInfos = MapManager.Instance.GetAllRooms();
            _roomsToCheck = allRoomInfos
                .OrderBy(r => Vector3.Distance(transform.position, r.CenterWorldPosition))
                .ToList();
            roomSearchStateDic = new Dictionary<RoomType, bool>();
            foreach(var info in allRoomInfos)
            {
                roomSearchStateDic.Add(info.RoomType, false);
            }
            roomSearchStateDic[RoomType.Corridor] = true;
            //InitSkills();


            behaviorTree.Start();
            agent.enabled = true;
        }
        public void Discard()
        {
            //回收状态机
            //stateMachine.ObjectPushPool();

            enemyBuffHandler.Discard();
            //DiscardSkills();

            behaviorTree.DisableBehavior();
            behaviorTree = null;

            _roomsToCheck.Clear();
            _roomsToCheck = null;
            roomSearchStateDic.Clear();
            roomSearchStateDic = null;
        }
        #endregion
        

        #region 感知判断

        public bool PlayerInSight()
        {
            if (player == null) return false;

            // 距离检查
            Vector3 dirToPlayer = player.position - transform.position;
            float distanceToPlayer = dirToPlayer.magnitude;
            if (distanceToPlayer > enemyBaseData.VisionRange)
            {
                SetVisionIndicator(false);
                return false;
            }
            // 扇形角度检查
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayer > enemyBaseData.VisionAngle / 2f)
            {
                SetVisionIndicator(false);
                return false;
            }

            // TODO:视线遮挡检查(视线被挡住也不算看到玩家）
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, dirToPlayer.normalized, out hit, distanceToPlayer))
            //{
            //    if (hit.collider.transform != player)
            //    {
            //        // 有物体遮挡玩家
            //        return false;
            //    }
            //}
            SetVisionIndicator(true);
            return true;
        }

        public bool PlayerInHearing()
        {
            if (player == null) return false;

            //if ((player.position - transform.position).magnitude <= enemyBaseData.HearingRange)
            //{
            //    targetPos = player.position;
            //    return true;
            //}
            //return false;
            if (hearingGO != null) return true;
            return false;
        }

        #endregion

        #region 受击
        public void TakeDamage(DamageInfo damageInfo)
        {
            // 现有伤害处理逻辑...
            curSanity = Mathf.Clamp(curSanity - damageInfo.damage, 0, enemyBaseData.MaxSanity);
            enemyBuffHandler.AddBuff(damageInfo.buffInfo);
            //是否进入眩晕状态
            if(curSanity == 0)
            {
                SetDizzyState(true);
            }
        }

        public void SetDizzyState(bool newState)
        {
            IsDizzying = newState;
            if(!IsDizzying)
            {
                curSanity = enemyBaseData.MaxSanity;
            }
        }
        public bool CheckDizzyState()
        {
            return IsDizzying;
        }
       

     

        #endregion

        #region 房间

        private void UpdateCurrentRoomType()
        {
            if (MapManager.Instance.TryGetRoomTypeAtWorldPos(transform.position, out var roomType))
            {
                _currentRoomType = roomType;
            }
        }

        #endregion

        

        

        #region 行为树封装方法

        /// <summary>
        /// 追击玩家
        /// </summary>
        public void ChasePlayer()
        {
            Agent.speed = enemyBaseData.ChaseSpeed;
            targetPos = player.transform.position;
            Agent.SetDestination(targetPos);
        }
        #endregion

        /// <summary>
        /// 是否检查了所有的房间
        /// </summary>
        /// <returns></returns>
        public bool CheckAllRooms()
        {
            foreach(var pair in roomSearchStateDic)
            {
                if (pair.Value == false) return false;
            }
            return true;
        }

        /// <summary>
        /// 去检查最近的未检查过的房间
        /// </summary>
        public RoomType GoNearestUnCheckRoom()
        {
            _roomsToCheck.OrderBy(r => Vector3.Distance(transform.position, r.CenterWorldPosition))
                .ToList();
            targetPos = Vector3.zero;
            RoomType nearestRoonType = RoomType.Corridor;
            foreach(var room in _roomsToCheck)
            {
                if(roomSearchStateDic[room.RoomType])
                {
                    continue;
                }
                targetPos = room.CenterWorldPosition;
                nearestRoonType = room.RoomType;
                break;
            }
            Agent.speed = enemyBaseData.MoveSpeed;
            Agent.SetDestination(targetPos);
            return nearestRoonType;
        }
        public void ResetAllRoomsCheckState()
        {
            var roomTypeList = new List<RoomType>(roomSearchStateDic.Keys);
            for (int i = 0; i < roomTypeList.Count; i++)//不要foreach里面修改 会报错
            {
                var roomType = roomTypeList[i];
                // 现在可以安全地修改roomCheckStates集合
                SetRoomCheckState(roomType,false);
            }

        }
        public void SetRoomCheckState(RoomType roomType,bool state)
        {
            roomSearchStateDic[roomType] = state;
        }

        public bool CheckArriveDestination(bool checkHearing = false)
        {
            if(checkHearing)
            {
                hearingGO = null;
            }
            return Vector3.Distance(transform.position, targetPos) < 0.5f;
        }

        public bool CheckCatchPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position) < 1f;
        }

        public void GoCheckHearPoint()
        {
            Agent.speed = enemyBaseData.MoveSpeed;
            targetPos = hearingGO.transform.position;
            Agent.SetDestination(targetPos);
        }


        private void SetVisionIndicator(bool seePlayer)
        {
            VisionIndicatorRenderer.material.SetColor("_Color", seePlayer ? SeePlayerColor : NoSeePlayerColor);
        }
        public void StopNav()
        {
            agent.isStopped = true;
        }
        public void StartNav()
        {
            agent.isStopped = false;
        }

        public void ProduceSound(float range) { }

        public void ReceiveSound(GameObject creator)
        {
            hearingGO = creator;
        }
        #region Gizoms


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyBaseData.VisionRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyBaseData.HearingRange);
        }

        #endregion
    }
}