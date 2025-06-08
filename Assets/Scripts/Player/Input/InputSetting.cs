using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 输入数据预处理
/// </summary>
public class InputSettings : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputActionAsset inputActionAsset;
    private InputAction interactionAction;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction runAction;
    private InputAction useAction;
    private InputAction throwAction;
    private InputAction bagAction;


    public event Action OnInteractionPress;

    private void Awake()
    {
        //输入模块获取
        try
        {
            playerInput = GetComponent<PlayerInput>();
            inputActionAsset = playerInput.actions;
            moveAction = inputActionAsset.FindAction("Move");
            interactionAction = inputActionAsset.FindAction("Interaction");
            dashAction = inputActionAsset.FindAction("Dash");
            runAction = inputActionAsset.FindAction("Run");
            useAction = inputActionAsset.FindAction("Use");
            throwAction = inputActionAsset.FindAction("Throw");
            bagAction = inputActionAsset.FindAction("Bag");


            interactionAction.performed += OnInteractionActionPerformed;
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法获取输入配置文件" + e);
            throw;
        }
    }

    /// <summary>
    /// 获取移动输入指向
    /// </summary>
    /// <returns></returns>
    public Vector2 MoveDir()
    {
        Vector2 inputDir = moveAction.ReadValue<Vector2>();
        return inputDir.normalized;
    }

    /// <summary>
    /// 是否按下冲刺键
    /// </summary>
    /// <returns></returns>
    public virtual bool GetDashDown() => dashAction.WasPressedThisFrame();

    /// <summary>
    /// 获取是否按住奔跑输入
    /// </summary>
    /// <returns></returns>
    public virtual bool GetIfRun() => runAction.IsPressed();

    public virtual bool GetRunUp() => runAction.WasReleasedThisFrame();

    /// <summary>
    /// 是否按下使用键
    /// </summary>
    /// <returns></returns>
    public virtual bool GetUseDonw() => useAction.WasPerformedThisFrame();

    /// <summary>
    /// 是否按下投掷键
    /// </summary>
    /// <returns></returns>
    public virtual bool GetThrowDown() => throwAction.WasPerformedThisFrame();

    public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();

    public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();


    /// <summary>
    /// 交互键被按下时触发
    /// </summary>
    /// <param name="context"></param>
    private void OnInteractionActionPerformed(InputAction.CallbackContext context)
    {
        // 当事件被触发时，通知所有订阅者
        if (OnInteractionPress != null)
        {
            OnInteractionPress.Invoke();
        }
    }
}