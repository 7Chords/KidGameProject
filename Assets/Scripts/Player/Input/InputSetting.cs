using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KidGame.Core
{
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
        private InputAction mouseWheelAction;

        // ���������¼�
        public event Action OnInteractionPress;
        public event Action OnDashPress;
        public event Action OnRunPress;
        public event Action OnRunRelease;
        public event Action OnUsePress;
        public event Action OnThrowPress;
        public event Action OnBagPress;
        public event Action OnRecyclePress;

        private void Awake()
        {
            try
            {
                playerInput = GetComponent<PlayerInput>();
                inputActionAsset = playerInput.actions;

                // ��ȡ�������붯��
                moveAction = inputActionAsset.FindAction("Move");
                interactionAction = inputActionAsset.FindAction("Interaction");
                dashAction = inputActionAsset.FindAction("Dash");
                runAction = inputActionAsset.FindAction("Run");
                useAction = inputActionAsset.FindAction("Use");
                throwAction = inputActionAsset.FindAction("Throw");
                bagAction = inputActionAsset.FindAction("Bag");
                recycleAction = inputActionAsset.FindAction("Recycle");
                mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
                // �����������¼�
                interactionAction.performed += OnInteractionActionPerformed;
                dashAction.performed += OnDashActionPerformed;
                runAction.performed += OnRunActionPerformed;
                runAction.canceled += OnRunActionCanceled;
                useAction.performed += OnUseActionPerformed;
                throwAction.performed += OnThrowActionPerformed;
                bagAction.performed += OnBagActionPerformed;
                recycleAction.performed += OnRecycleActionPerformed;
            }
            catch (Exception e)
            {
                Debug.LogWarning("�޷���ȡ���������ļ�" + e);
                throw;
            }
        }

        private void OnDestroy()
        {
            // ������������¼�
            interactionAction.performed -= OnInteractionActionPerformed;
            dashAction.performed -= OnDashActionPerformed;
            runAction.performed -= OnRunActionPerformed;
            runAction.canceled -= OnRunActionCanceled;
            useAction.performed -= OnUseActionPerformed;
            throwAction.performed -= OnThrowActionPerformed;
            bagAction.performed -= OnBagActionPerformed;
            recycleAction.performed -= OnRecycleActionPerformed;
        }

        /// <summary>
        /// ��ȡ�ƶ�����ָ��
        /// </summary>
        public Vector2 MoveDir()
        {
            Vector2 inputDir = moveAction.ReadValue<Vector2>();
            return inputDir.normalized;
        }

        /// <summary>
        /// ������ֵ
        /// </summary>
        /// <returns></returns>
        public float MouseWheelValue()
        {
            float val = mouseWheelAction.ReadValue<float>();
            //inputaction������Ѿ������˷�Χ[-1,1]
            return val;
        }

        public virtual bool GetDashDown() => dashAction.WasPressedThisFrame();
        public virtual bool GetIfRun() => runAction.IsPressed();
        public virtual bool GetRunUp() => runAction.WasReleasedThisFrame();
        public virtual bool GetUseDonw() => useAction.WasPerformedThisFrame();
        public virtual bool GetThrowDown() => throwAction.WasPerformedThisFrame();
        public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();
        public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();
        public virtual bool GetRecycleDown() => recycleAction.WasPerformedThisFrame();

        #region �����¼�����

        private void OnInteractionActionPerformed(InputAction.CallbackContext context)
        {
            OnInteractionPress?.Invoke();
        }

        private void OnDashActionPerformed(InputAction.CallbackContext context)
        {
            OnDashPress?.Invoke();
        }

        private void OnRunActionPerformed(InputAction.CallbackContext context)
        {
            OnRunPress?.Invoke();
        }

        private void OnRunActionCanceled(InputAction.CallbackContext context)
        {
            OnRunRelease?.Invoke();
        }

        private void OnUseActionPerformed(InputAction.CallbackContext context)
        {
            OnUsePress?.Invoke();
        }

        private void OnThrowActionPerformed(InputAction.CallbackContext context)
        {
            OnThrowPress?.Invoke();
        }

        private void OnBagActionPerformed(InputAction.CallbackContext context)
        {
            OnBagPress?.Invoke();
        }

        private void OnRecycleActionPerformed(InputAction.CallbackContext context)
        {
            OnRecyclePress?.Invoke();
        }

        #endregion

        #region ������
        /// <summary>
        /// ��ȡ���õļ�λ
        /// </summary>
        /// <param name="actionName">��������</param>
        /// <param name="controlScheme">���Ʒ������ƣ�Ĭ��Ϊ��ǰ����ķ���</param>
        /// <param name="bindingIndex">��������Ĭ��Ϊ��һ����</param>
        /// <returns>��ʽ����ļ�λ��ʾ�ַ���</returns>
        public string GetSettingKey(InputActionType actionType, int controlTypeIndex)
        {
            if (inputActionAsset == null)
            {
                Debug.LogError("InputActionAssetδ��ʼ��");
                return string.Empty;
            }

            var action = inputActionAsset.FindAction(actionType.ToString());
            if (action == null)
            {
                Debug.LogError($"δ�ҵ���Ϊ '{actionType}' �Ķ���");
                return string.Empty;
            }

            return action.bindings[controlTypeIndex].ToDisplayString();
        }
        #endregion
    }
}

