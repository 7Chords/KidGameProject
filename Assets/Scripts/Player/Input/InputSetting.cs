using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KidGame.Core
{
    /// <summary>
    /// ?????????????
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
        private InputAction pickAction;
        private InputAction mouseWheelAction;

        public event Action OnInteractionPress;
        public event Action OnDashPress;
        public event Action OnRunPress;
        public event Action OnRunRelease;
        public event Action OnUsePress;
        public event Action OnThrowPress;
        public event Action OnBagPress;
        public event Action OnPickPress;

        private void Awake()
        {
            try
            {
                playerInput = GetComponent<PlayerInput>();
                inputActionAsset = playerInput.actions;

                // ?????????????
                moveAction = inputActionAsset.FindAction("Move");
                interactionAction = inputActionAsset.FindAction("Interaction");
                dashAction = inputActionAsset.FindAction("Dash");
                runAction = inputActionAsset.FindAction("Run");
                useAction = inputActionAsset.FindAction("Use");
                throwAction = inputActionAsset.FindAction("Throw");
                bagAction = inputActionAsset.FindAction("Bag");
                pickAction = inputActionAsset.FindAction("Pick");
                mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
                // ?????????????
                interactionAction.performed += OnInteractionActionPerformed;
                dashAction.performed += OnDashActionPerformed;
                runAction.performed += OnRunActionPerformed;
                runAction.canceled += OnRunActionCanceled;
                useAction.performed += OnUseActionPerformed;
                throwAction.performed += OnThrowActionPerformed;
                bagAction.performed += OnBagActionPerformed;
                pickAction.performed += OnPickActionPerformed;
            }
            catch (Exception e)
            {
                Debug.LogWarning("?????????????????" + e);
                throw;
            }
        }

        private void OnDestroy()
        {
            // ??????????????
            interactionAction.performed -= OnInteractionActionPerformed;
            dashAction.performed -= OnDashActionPerformed;
            runAction.performed -= OnRunActionPerformed;
            runAction.canceled -= OnRunActionCanceled;
            useAction.performed -= OnUseActionPerformed;
            throwAction.performed -= OnThrowActionPerformed;
            bagAction.performed -= OnBagActionPerformed;
            pickAction.performed -= OnPickActionPerformed;
        }

        /// <summary>
        /// ?????????????
        /// </summary>
        public Vector2 MoveDir()
        {
            Vector2 inputDir = moveAction.ReadValue<Vector2>();
            return inputDir.normalized;
        }

        /// <summary>
        /// ???????
        /// </summary>
        /// <returns></returns>
        public float MouseWheelValue()
        {
            float val = mouseWheelAction.ReadValue<float>();
            //inputaction???????????????¦¶[-1,1]
            return val;
        }

        public virtual bool GetDashDown() => dashAction.WasPressedThisFrame();
        public virtual bool GetIfRun() => runAction.IsPressed();
        public virtual bool GetRunUp() => runAction.WasReleasedThisFrame();
        public virtual bool GetUseDonw() => useAction.WasPerformedThisFrame();
        public virtual bool GetThrowDown() => throwAction.WasPerformedThisFrame();
        public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();
        public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();
        public virtual bool GetPickDown() => pickAction.WasPerformedThisFrame();

        #region ???????????

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

        private void OnPickActionPerformed(InputAction.CallbackContext context)
        {
            OnPickPress?.Invoke();
        }

        #endregion

        #region ??????

        /// <summary>
        /// ?????????¦Ë
        /// </summary>
        /// <param name="actionName">????????</param>
        /// <param name="controlScheme">???????????????????????????</param>
        /// <param name="bindingIndex">???????????????????</param>
        /// <returns>?????????¦Ë????????</returns>
        public string GetSettingKey(InputActionType actionType, int controlTypeIndex)
        {
            if (inputActionAsset == null)
            {
                Debug.LogError("InputActionAsset¦Ä?????");
                return string.Empty;
            }

            var action = inputActionAsset.FindAction(actionType.ToString());
            if (action == null)
            {
                Debug.LogError($"¦Ä?????? '{actionType}' ?????");
                return string.Empty;
            }

            return action.bindings[controlTypeIndex].ToDisplayString();
        }

        #endregion
    }
}