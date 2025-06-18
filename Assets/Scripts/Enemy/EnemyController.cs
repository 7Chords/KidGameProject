using KidGame.Core;
using UnityEngine;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// ��������߼���״̬���Լ���֪�ж�
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour, IStateMachineOwner,IDamageable
    {
        [SerializeField] private EnemyBaseData enemyBaseData;
        [Tooltip("Ѳ�ߵ��б���ѡ��Ϊ��ʱ����ԭ�� Idle �� Patrol ѭ��")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 2.0f;

        private StateMachine stateMachine;
        private Rigidbody rb;
        private Transform player;
    
        public EnemyBaseData EnemyBaseData => enemyBaseData;
        public Rigidbody Rb => rb;

        // Ѳ��״̬��Ҫ����ʱ����
        [HideInInspector] public int CurrentPatrolIndex;
        [HideInInspector] public float PatrolTimer;

        private BuffHandler enemyBuffHandler;

        #region ��������
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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


        public Transform Player => player;
        public Transform[] PatrolPoints => patrolPoints;
        public float PatrolWaitTime => patrolWaitTime;
        #endregion

        public void TakeDamage(DamageInfo damageInfo)
        {

        }
    }
}
