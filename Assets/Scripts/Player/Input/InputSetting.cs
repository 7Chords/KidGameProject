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

            interactionAction.performed += OnInteractionActionPerformed;
        }
        catch (Exception e)
        {
            Debug.LogWarning("�޷���ȡ���������ļ�" + e);
            throw;
        }
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