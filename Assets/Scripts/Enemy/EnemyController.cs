using System.Collections;
using UnityEngine;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// ��������߼���״̬���Լ���֪�ж�
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour, IStateMachineOwner, IDamageable
    {
        [SerializeField] private EnemyBaseData enemyBaseData;

        [Tooltip("Ѳ�ߵ��б�")] [SerializeField]
        private Transform[] patrolPoints;

        [SerializeField] private float patrolWaitTime = 2.0f;

        private StateMachine stateMachine;
        private Rigidbody rb;
        private Transform player;

        public EnemyBaseData EnemyBaseData => enemyBaseData;
        public Rigidbody Rb => rb;

        private BuffHandler enemyBuffHandler;
        
        #region Ѳ��

        // Ѳ��״̬��Ҫ����ʱ����
        [HideInInspector] public int CurrentPatrolIndex;
        [HideInInspector] public float PatrolTimer;
        public Transform Player => player;
        public Transform[] PatrolPoints => patrolPoints;
        public float PatrolWaitTime => patrolWaitTime;

        #endregion

        #region ���ڷ���

        private RoomType _currentRoomType;
        public RoomType CurrentRoomType => _currentRoomType;

        #endregion

        #region ��������

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
            ChangeState(EnemyState.Idle); // ��ʼ״̬

            enemyBuffHandler = new BuffHandler();
            enemyBuffHandler.Init();
        }

        #endregion

        #region ״̬����װ

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
            //����״̬��
            stateMachine.ObjectPushPool();

            enemyBuffHandler.Discard();
        }

        #endregion

        #region ��֪�ж�

        public bool PlayerInSight()
        {
            if (player == null) return false;
            Vector3 dir = player.position - transform.position;
            if (dir.magnitude > enemyBaseData.VisionRange) return false;

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