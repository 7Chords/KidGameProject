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
    public class EnemyController : MonoBehaviour, IStateMachineOwner, IDamageable
    {
        [SerializeField]
        private List<string> randomDamgeSfxList;
        public List<string> RandomDamgeSfxList { get => randomDamgeSfxList; set { randomDamgeSfxList = value; } }

        [SerializeField]
        private ParticleSystem damagePartical;
        public ParticleSystem DamagePartical { get => damagePartical; set { damagePartical = value; } }




        [SerializeField] private EnemyBaseData enemyBaseData;
        public EnemyBaseData EnemyBaseData => enemyBaseData;

        //private StateMachine stateMachine;

        private Transform player;



        private Rigidbody rb;
        public Rigidbody Rb => rb;

        private Animator animator;
        public Animator Animator => animator;

        private NavMeshAgent agent;
        public NavMeshAgent Agent => agent;

        private BuffHandler enemyBuffHandler;

        private BehaviorTree behaviorTree;

        private Dictionary<RoomType, bool> roomSearchStateDic;//������������ֵ�

        private List<RoomInfo> _roomsToCheck = new List<RoomInfo>();

        private Vector3 targetPos;

        #region ��Ŀ������

        public string _currentTargetItemId;
        
        #endregion
        
        #region ���Ǳ���

        private int _currentHealth;

        #endregion

        #region Ѳ��
        
        [HideInInspector] public int CurrentPatrolIndex;
        [HideInInspector] public float PatrolTimer;
        public Transform Player => player;

        [Tooltip("Ѳ�ߵ��б�")] [SerializeField] private Transform[] patrolPoints;
        public Transform[] PatrolPoints => patrolPoints;
        public float PatrolWaitTime => patrolWaitTime;
        [SerializeField] private float patrolWaitTime = 2.0f;

        #endregion

        #region ���ڷ���

        private RoomType _currentRoomType;
        public RoomType CurrentRoomType => _currentRoomType;


        #endregion

        #region ����

        private List<ActiveSkillInstance> _activeSkillInstances = new List<ActiveSkillInstance>();
        private List<PassiveSkillSO> _appliedPassiveSkills = new List<PassiveSkillSO>();
        private class ActiveSkillInstance
        {
            public ActiveSkillSO skillSO;
            public float currentCooldown;
        }
        
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
            UpdateSkillCooldowns();
            CheckSkillTriggers();
        }

        public void Init(EnemyBaseData enemyData)
        {
            enemyBaseData = enemyData;

            //stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            //stateMachine.Init(this);
            //ChangeState(EnemyState.Idle); // ��ʼ״̬
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
        }
        public void Discard()
        {
            //����״̬��
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

        #region ״̬����װ(DELETE)

        //public bool ChangeState(EnemyState newState)
        //{
        //    switch (newState)
        //    {
        //        case EnemyState.Idle:
        //            return stateMachine.ChangeState<EnemyIdleState>((int)newState);
        //        case EnemyState.Patrol:
        //            return stateMachine.ChangeState<EnemyPatrolState>((int)newState);
        //        case EnemyState.Attack:
        //            return stateMachine.ChangeState<EnemyAttackState>((int)newState);
        //        case EnemyState.SearchTargetRoom:
        //            return stateMachine.ChangeState<EnemySearchTargetRoomState>((int)newState);
        //        default:
        //            return false;
        //    }
        //}


        #endregion

        #region ��֪�ж�

        public bool PlayerInSight()
        {
            if (player == null) return false;
            Vector3 dir = player.position - transform.position;
            if (dir.magnitude > enemyBaseData.VisionRange) return false;
            //TODO:���������ж�
            // ����������������߼�⣬�ų����ڵ�������
            return true;
        }

        public bool PlayerInHearing()
        {
            if (player == null) return false;
            return (player.position - transform.position).magnitude <= enemyBaseData.HearingRange;
        }

        #endregion

        #region �ܻ�

        public void Stun(float duration)
        {
            StartCoroutine(StunRoutine(duration));
        }

        private IEnumerator StunRoutine(float duration)
        {
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(duration);
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

        #region ����
        private void InitSkills()
        {
            // ��ʼ����������ʵ��
            foreach (var skillSO in enemyBaseData.activeSkills)
            {
                _activeSkillInstances.Add(new ActiveSkillInstance
                {
                    skillSO = skillSO,
                    currentCooldown = 0f
                });

                // ����ʱ��������
                if (skillSO.triggerCondition == SkillTriggerCondition.OnTimer)
                {
                    StartCoroutine(TimerSkillRoutine(skillSO));
                }

                // ��������ʱ��������
                if (skillSO.triggerCondition == SkillTriggerCondition.OnSpawn)
                {
                    TryTriggerSkill(skillSO);
                }
            }

            // Ӧ�ñ�������
            foreach (var skillSO in enemyBaseData.passiveSkills)
            {
                skillSO.Apply(this);
                _appliedPassiveSkills.Add(skillSO);
            }
        }

        private void DiscardSkills()
        {
            // �Ƴ����б�������Ч��
            foreach (var skillSO in _appliedPassiveSkills)
            {
                skillSO.Remove(this);
            }
        }
        private void UpdateSkillCooldowns()
        {
            foreach (var skillInstance in _activeSkillInstances)
            {
                if (skillInstance.currentCooldown > 0)
                {
                    skillInstance.currentCooldown -= Time.deltaTime;
                }
            }
        }

        private void CheckSkillTriggers()
        {
            if (player == null) return;

            foreach (var skillInstance in _activeSkillInstances)
            {
                if (skillInstance.currentCooldown > 0) continue;

                var skillSO = skillInstance.skillSO;

                switch (skillSO.triggerCondition)
                {
                    case SkillTriggerCondition.OnPlayerInRange:
                        if (Vector3.Distance(transform.position, player.position) <= skillSO.triggerRange)
                        {
                            TryTriggerSkill(skillSO);
                        }

                        break;
                }
            }
        }

        private IEnumerator TimerSkillRoutine(ActiveSkillSO skillSO)
        {
            while (true)
            {
                yield return new WaitForSeconds(skillSO.timerInterval);
                TryTriggerSkill(skillSO);
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            // �����˺������߼�...

            // ��鱻����ʱ�����ļ���
            foreach (var skillInstance in _activeSkillInstances)
            {
                var skillSO = skillInstance.skillSO;
                if (skillSO.triggerCondition == SkillTriggerCondition.OnHit &&
                    skillInstance.currentCooldown <= 0)
                {
                    TryTriggerSkill(skillSO);
                }
            }

            // ����Ѫ�������ļ���
            if (_currentHealth / enemyBaseData.MaxSanity <= 0.3f)
            {
                foreach (var skillInstance in _activeSkillInstances)
                {
                    var skillSO = skillInstance.skillSO;
                    if (skillSO.triggerCondition == SkillTriggerCondition.OnLowHealth &&
                        skillInstance.currentCooldown <= 0)
                    {
                        TryTriggerSkill(skillSO);
                    }
                }
            }
        }

        private void TryTriggerSkill(ActiveSkillSO skillSO)
        {
            var skillInstance = _activeSkillInstances.Find(s => s.skillSO == skillSO);
            if (skillInstance == null || skillInstance.currentCooldown > 0) return;

            skillSO.Execute(this);
            skillInstance.currentCooldown = skillSO.cooldown;
        }

        #endregion

        #region ����

        public void OnAttack()
        {
            foreach (var skillInstance in _activeSkillInstances)
            {
                var skillSO = skillInstance.skillSO;
                if (skillSO.triggerCondition == SkillTriggerCondition.OnAttack &&
                    skillInstance.currentCooldown <= 0)
                {
                    TryTriggerSkill(skillSO);
                }
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
            Agent.SetDestination(player.transform.position);
        }
        #endregion

        /// <summary>
        /// �Ƿ��������еķ���
        /// </summary>
        /// <returns></returns>
        public bool CheckAllRooms()
        {
            foreach(var pair in roomSearchStateDic)
            {
                Debug.Log(pair.Key + ":" + pair.Value);
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
            Debug.Log(nearestRoonType);
            Agent.speed = enemyBaseData.MoveSpeed;
            Agent.SetDestination(targetPos);
            return nearestRoonType;
        }
        public void ResetAllRoomsCheckState()
        {
            foreach(var key in roomSearchStateDic.Keys)
            {
                SetRoomCheckState(key, false);
            }
        }
        public void SetRoomCheckState(RoomType roomType,bool state)
        {
            roomSearchStateDic[roomType] = state;
        }

        public bool CheckArriveDestination()
        {
            return Vector3.Distance(transform.position, targetPos) < 0.5f;
        }
    }
}