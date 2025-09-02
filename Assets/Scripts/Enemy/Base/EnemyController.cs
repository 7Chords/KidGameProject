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
    /// ��װ������ر����뷽��
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour, IStateMachineOwner, IDamageable, ISoundable
    {

        #region Comp

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Animator animator;
        public Animator Animator => animator;

        private NavMeshAgent agent;
        public NavMeshAgent Agent => agent;

        private BuffHandler enemyBuffHandler;

        private BehaviorTree behaviorTree;
        
        private EnemyHUDController enemyHUDController; // ���˵�hud

        #endregion

        #region ��Ϊ�����ò���

        public float testTime;

        #endregion

        [SerializeField] private EnemyBaseData enemyBaseData;
        public EnemyBaseData EnemyBaseData => enemyBaseData;

        private PlayerController playerController;

        private Transform player;

        public Transform Player
        {
            get { return player; }
        }

        private Dictionary<RoomType, bool> roomSearchStateDic; //������������ֵ�

        private List<RoomInfo> _roomsToCheck = new List<RoomInfo>();

        private Vector3 targetPos;

        [Header("VisionIndicatorSetting")]
        public Renderer VisionIndicatorRenderer;
        public Color NoSeePlayerColor;
        public Color SeePlayerColor;

        private float curSanity; // ���˵�����
        public bool IsDizzying;

        private GameObject hearingGO;

        #region ��Ŀ������

        public string _currentTargetItemId;

        #endregion

        #region ���ڷ���

        private RoomType _currentRoomType;
        public RoomType CurrentRoomType => _currentRoomType;

        #endregion

        #region ��������

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
            
            // if (enemyHUDController != null)
            // {
            //     enemyHUDController.UpdateEnemySanity(this, curSanity, enemyBaseData.MaxSanity);
            //     enemyHUDController.UpdateEnemyBuffs(this);
            // }
        }

        public void Init(EnemyBaseData enemyData)
        {
            enemyBaseData = enemyData;
            curSanity = enemyBaseData.MaxSanity;

            playerController = FindObjectOfType<PlayerController>();

            player = playerController.gameObject.transform;

            enemyBuffHandler = new BuffHandler();
            enemyBuffHandler.Init();

            // ��ȡ�򴴽�hud������
            enemyHUDController = FindObjectOfType<EnemyHUDController>();
            if (enemyHUDController == null)
            {
                GameObject hudControllerObj = new GameObject("EnemyHUDController");
                enemyHUDController = hudControllerObj.AddComponent<EnemyHUDController>();
            }
    
            // ע�ᵽhud
            enemyHUDController.RegisterEnemy(this);
            

            List<RoomInfo> allRoomInfos = MapManager.Instance.GetAllRooms();
            _roomsToCheck = allRoomInfos
                .OrderBy(r => Vector3.Distance(transform.position, r.CenterWorldPosition))
                .ToList();
            roomSearchStateDic = new Dictionary<RoomType, bool>();
            foreach (var info in allRoomInfos)
            {
                roomSearchStateDic.Add(info.RoomType, false);
            }

            roomSearchStateDic[RoomType.Corridor] = true;

            behaviorTree.Start();
            agent.enabled = true;
        }

        public void Discard()
        {
            if (enemyHUDController != null)
            {
                enemyHUDController.UnregisterEnemy(this);
            }
            
            enemyBuffHandler.Discard();

            behaviorTree.DisableBehavior();
            behaviorTree = null;

            _roomsToCheck.Clear();
            _roomsToCheck = null;
            roomSearchStateDic.Clear();
            roomSearchStateDic = null;

            Destroy(gameObject);
        }

        #endregion

        #region ��֪�ж�

        public bool PlayerInSight()
        {
            if (player == null) return false;

            // ������
            Vector3 dirToPlayer = player.position - transform.position;
            float distanceToPlayer = dirToPlayer.magnitude;
            if (distanceToPlayer > enemyBaseData.VisionRange)
            {
                SetVisionIndicator(false);
                return false;
            }

            // ���νǶȼ��
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayer > enemyBaseData.VisionAngle / 2f)
            {
                SetVisionIndicator(false);
                return false;
            }

            SetVisionIndicator(true);
            return true;
        }

        public bool PlayerInHearing()
        {
            if (player == null) return false;
            // Ŀǰ����û����ϵͳ����ʱ��ôд
            if ((player.position - transform.position).magnitude <=
                enemyBaseData.HearingRange && (playerController.IsPlayerState(PlayerState.Dash) ||
                                               playerController.InputSettings.GetIfRun()))
            {
                targetPos = player.position;
                return true;
            }

            return false;
        }

        #endregion

        #region �ܻ�

        public void TakeDamage(DamageInfo damageInfo)
        {
            curSanity = Mathf.Clamp(curSanity - damageInfo.damage, 0, enemyBaseData.MaxSanity);
            enemyBuffHandler.AddBuff(damageInfo.buffInfo);

            // ֪ͨhud����
            MsgCenter.SendMsg(MsgConst.ON_ENEMY_SANITY_CHG, this, curSanity, enemyBaseData.MaxSanity);
            MsgCenter.SendMsg(MsgConst.ON_ENEMY_BUFF_CHG, this);

            if (curSanity == 0)
            {
                SetDizzyState(true);
            }
        }

        public void SetDizzyState(bool newState)
        {
            IsDizzying = newState;
            if (!IsDizzying)
            {
                curSanity = enemyBaseData.MaxSanity;
            }

            // �㲥ѣ��״̬
            MsgCenter.SendMsg(MsgConst.ON_ENEMY_DIZZY, this, IsDizzying);
        }


        public bool CheckDizzyState()
        {
            return IsDizzying;
        }

        #endregion

        #region ����

        private void UpdateCurrentRoomType()
        {
            if (MapManager.Instance.TryGetRoomTypeAtWorldPos(transform.position, out var roomType))
            {
                _currentRoomType = roomType;
            }
        }

        #endregion

        #region ��Ϊ����װ����

        /// <summary>
        /// ׷�����
        /// </summary>
        public void ChasePlayer()
        {
            Agent.speed = enemyBaseData.ChaseSpeed;
            targetPos = player.transform.position;
            Agent.SetDestination(targetPos);
        }

        /// <summary>
        /// ����Ŀ�굽ĳ����
        /// </summary>
        public void SetMoveTarget(Vector3 position)
        {
            Agent.speed = enemyBaseData.MoveSpeed;
            targetPos = position;
            Agent.SetDestination(targetPos);
        }

        #endregion

        /// <summary>
        /// �Ƿ��������еķ���
        /// </summary>
        /// <returns></returns>
        public bool CheckAllRooms()
        {
            foreach (var pair in roomSearchStateDic)
            {
                if (pair.Value == false) return false;
            }

            return true;
        }

        /// <summary>
        /// ȥ��������δ�����ķ���
        /// </summary>
        public RoomType GoNearestUnCheckRoom()
        {
            _roomsToCheck.OrderBy(r => Vector3.Distance(transform.position, r.CenterWorldPosition))
                .ToList();
            targetPos = Vector3.zero;
            RoomType nearestRoonType = RoomType.Corridor;
            foreach (var room in _roomsToCheck)
            {
                if (roomSearchStateDic[room.RoomType])
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
            for (int i = 0; i < roomTypeList.Count; i++) //��Ҫforeach�����޸� �ᱨ��
            {
                var roomType = roomTypeList[i];
                // ���ڿ��԰�ȫ���޸�roomCheckStates����
                SetRoomCheckState(roomType, false);
            }
        }

        public void SetRoomCheckState(RoomType roomType, bool state)
        {
            roomSearchStateDic[roomType] = state;
        }

        public bool CheckArriveDestination(bool checkHearing = false)
        {
            if (checkHearing)
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

        public void ProduceSound(float range)
        {
        }

        public void ReceiveSound(GameObject creator)
        {
            hearingGO = creator;
        }
        
        public List<BuffInfo> GetBuffList()
        {
            return enemyBuffHandler.buffList;
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