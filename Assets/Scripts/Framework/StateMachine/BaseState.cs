/// <summary>
/// ״̬����
/// </summary>
public abstract class StateBase
{
    protected StateMachine stateMachine;
    /// <summary>
    /// ��ʼ��״̬
    /// ֻ��״̬��һ�δ���ʱִ��
    /// </summary>
    /// <param name="owner">����</param>
    /// <param name="stateType">״̬����ö�ٵ�ֵ</param>
    /// <param name="stateMachine">����״̬��</param>
    public virtual void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// ����ʼ��
    /// ����ʹ��ʱ�򣬷Żض����ʱ����
    /// ��һЩ�����ÿգ���ֹ���ܱ�GC
    /// </summary>
    public virtual void UnInit()
    {
        // �Żض����
        this.ObjectPushPool();
    }

    /// <summary>
    /// ״̬����
    /// ÿ�ν��붼��ִ��
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// ״̬�˳�
    /// </summary>
    public virtual void Exit() { }

    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }

}