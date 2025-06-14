using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ��������Ԥ����
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
    private InputAction recycleAction;

    public event Action OnInteractionPress;

    private void Awake()
    {
        //����ģ���ȡ
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
            recycleAction = inputActionAsset.FindAction("Recycle");

            interactionAction.performed += OnInteractionActionPerformed;
        }
        catch (Exception e)
        {
            Debug.LogWarning("�޷���ȡ���������ļ�" + e);
            throw;
        }
    }

    private void OnDestroy()
    {
        interactionAction.performed -= OnInteractionActionPerformed;
    }

    /// <summary>
    /// ��ȡ�ƶ�����ָ��
    /// </summary>
    /// <returns></returns>
    public Vector2 MoveDir()
    {
        Vector2 inputDir = moveAction.ReadValue<Vector2>();
        return inputDir.normalized;
    }

    /// <summary>
    /// �Ƿ��³�̼�
    /// </summary>
    /// <returns></returns>
    public virtual bool GetDashDown() => dashAction.WasPressedThisFrame();

    /// <summary>
    /// ��ȡ�Ƿ�ס��������
    /// </summary>
    /// <returns></returns>
    public virtual bool GetIfRun() => runAction.IsPressed();

    public virtual bool GetRunUp() => runAction.WasReleasedThisFrame();

    /// <summary>
    /// �Ƿ���ʹ�ü�
    /// </summary>
    /// <returns></returns>
    public virtual bool GetUseDonw() => useAction.WasPerformedThisFrame();

    /// <summary>
    /// �Ƿ���Ͷ����
    /// </summary>
    /// <returns></returns>
    public virtual bool GetThrowDown() => throwAction.WasPerformedThisFrame();

    /// <summary>
    /// �Ƿ��´򿪱�����
    /// </summary>
    /// <returns></returns>
    public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();

    /// <summary>
    /// �Ƿ��½�����
    /// </summary>
    /// <returns></returns>
    public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();

    /// <summary>
    /// �Ƿ��»��ռ�
    /// </summary>
    /// <returns></returns>
    public virtual bool GetRecycleDown() => recycleAction.WasPerformedThisFrame();

    /// <summary>
    /// ������������ʱ����
    /// </summary>
    /// <param name="context"></param>
    private void OnInteractionActionPerformed(InputAction.CallbackContext context)
    {
        // ���¼�������ʱ��֪ͨ���ж�����
        if (OnInteractionPress != null)
        {
            OnInteractionPress.Invoke();
        }
    }
}