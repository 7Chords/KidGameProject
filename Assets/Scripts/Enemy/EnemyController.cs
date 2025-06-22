using System.Collections;
using UnityEngine;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// 管理敌人逻辑、状态机以及感知判断
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour, IStateMachineOwner, IDamageable
    {
        [SerializeField] private EnemyBaseData enemyBaseData;

        [Tooltip("巡逻点列表")] [SerializeField]
        private Transform[] patrolPoints;

        [SerializeField] private float patrolWaitTime = 2.0f;

        private StateMachine stateMachine;
        private Rigidbody rb;
        private Transform player;

        public EnemyBaseData EnemyBaseData => enemyBaseData;
        public Rigidbody Rb => rb;

        private BuffHandler enemyBuffHandler;
        
        #region 巡逻

        // 巡逻状态需要的临时变量
        [HideInInspector] public int CurrentPatrolIndex;
        [HideInInspector] public float PatrolTimer;
        public Transform Player => player;
        public Transform[] PatrolPoints => patrolPoints;
        public float PatrolWaitTime => patrolWaitTime;

        #endregion

        #region 所在房间

        private RoomType _currentRoomType;
        public RoomType CurrentRoomType => _currentRoomType;

        #endregion

        #region 生命周期

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            UpdateCurrentRoomType();
        }

        public void Init(EnemyBaseData enemyData)
        {
            enemyBaseData = enemyData;

            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            player = FindObjectOfType<PlayerController>().gameObject.transform;
            ChangeState(EnemyState.Idle); // 初始状态

            enemyBuffHandler = new BuffHandler();
            enemyBuffHandler.Init();
        }

        #endregion

        #region 状态机包装

        public bool ChangeState(EnemyState newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    return stateMachine.ChangeState<EnemyIdleState>((int)newState);
                case EnemyState.Patrol:
                    return stateMachine.ChangeState<EnemyPatrolState>((int)newState);
                case EnemyState.Attack:
                    return stateMachine.ChangeState<EnemyAttackState>((int)newState);
                default:
                    return false;
            }
        }

        public void Discard()
        {
            //回收状态机
            stateMachine.ObjectPushPool();

            enemyBuffHandler.Discard();
        }

        #endregion

        #region 感知判断

        public bool PlayerInSight()
        {
            if (player == null) return false;
            Vector3 dir = player.position - transform.position;
            if (dir.magnitude > enemyBaseData.VisionRange) return false;

            // 可以在这里添加射线检测，排除被遮挡的物体
            return true;
        }

        public bool PlayerInHearing()
        {
            if (player == null) return false;
            return (player.position - transform.position).magnitude <= enemyBaseData.HearingRange;
        }

        #endregion

        #region 受击

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

        private void UpdateCurrentRoomType()
        {
            if (MapManager.Instance.TryGetRoomTypeAtWorldPos(transform.position, out var roomType))
            {
                _currentRoomType = roomType;
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
        }
    }
}