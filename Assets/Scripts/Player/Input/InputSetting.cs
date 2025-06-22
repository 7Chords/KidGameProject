using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KidGame.Core
{
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
        private InputAction recycleAction;
        private InputAction mouseWheelAction;

        // 所有输入事件
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

                // 获取所有输入动作
                moveAction = inputActionAsset.FindAction("Move");
                interactionAction = inputActionAsset.FindAction("Interaction");
                dashAction = inputActionAsset.FindAction("Dash");
                runAction = inputActionAsset.FindAction("Run");
                useAction = inputActionAsset.FindAction("Use");
                throwAction = inputActionAsset.FindAction("Throw");
                bagAction = inputActionAsset.FindAction("Bag");
                recycleAction = inputActionAsset.FindAction("Recycle");
                mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
                // 绑定所有输入事件
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
                Debug.LogWarning("无法获取输入配置文件" + e);
                throw;
            }
        }

        private void OnDestroy()
        {
            // 解绑所有输入事件
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
        /// 获取移动输入指向
        /// </summary>
        public Vector2 MoveDir()
        {
            Vector2 inputDir = moveAction.ReadValue<Vector2>();
            return inputDir.normalized;
        }

        /// <summary>
        /// 鼠标滚轮值
        /// </summary>
        /// <returns></returns>
        public float MouseWheelValue()
        {
            float val = mouseWheelAction.ReadValue<float>();
            //inputaction面板中已经限制了范围[-1,1]
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

        #region 输入事件处理

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
    }
}

