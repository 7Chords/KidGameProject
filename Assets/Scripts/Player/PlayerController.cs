using UnityEngine;


public class PlayerController : MonoBehaviour,IStateMachineOwner
{
    private InputSettings inputSettings;

    public InputSettings InputSettings => inputSettings;

    private CharacterController characterController;
    public CharacterController CharacterController => characterController;

    private Animator animator;

    private StateMachine stateMachine;
    private PlayerState playerState; // 玩家的当前状态

    public PlayerBaseData PlayerBaseData;
    void Start()
    {
        inputSettings = GetComponent<InputSettings>();
        characterController = GetComponent<CharacterController>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        stateMachine = PoolManager.Instance.GetObject<StateMachine>();
        stateMachine.Init(this);
        ChangeState(PlayerState.Idle);
    }

    public void Rotate(Vector3 dir)
    {
        if (dir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;
        }
    }


    /// <summary>
    /// 修改状态
    /// </summary>
    public void ChangeState(PlayerState playerState, bool reCurrstate = false)
    {
        this.playerState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<PlayerIdleState>((int)playerState, reCurrstate);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<PlayerMoveState>((int)playerState, reCurrstate);
                break;
            default:
                break;
        }
    }

    public void PlayerAnimation(string animationName)
    {
        animator.Play(animationName);
    }
}