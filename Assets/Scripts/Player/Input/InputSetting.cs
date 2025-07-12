using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KidGame.Core
{
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
        private InputAction gamePauseAction;

        public event Action OnInteractionPress;
        public event Action OnDashPress;
        public event Action OnRunPress;
        public event Action OnRunRelease;
        public event Action OnUsePress;
        public event Action OnThrowPress;
        public event Action OnBagPress;
        public event Action OnPickPress;
        public event Action OnMouseWheelPress;
        public event Action OnGamePause;

        private void Awake()
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
            pickAction = inputActionAsset.FindAction("Pick");
            mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
            gamePauseAction = inputActionAsset.FindAction("GamePause");

            interactionAction.performed += OnInteractionActionPerformed;
            dashAction.performed += OnDashActionPerformed;
            runAction.performed += OnRunActionPerformed;
            runAction.canceled += OnRunActionCanceled;
            useAction.performed += OnUseActionPerformed;
            throwAction.performed += OnThrowActionPerformed;
            bagAction.performed+= OnBagActionPerformed;
            pickAction.performed += OnPickActionPerformed;
            mouseWheelAction.performed += OnMouseWheelActionPerformed;
            gamePauseAction.performed += OnGamePauseActionPerformed;
        }

        private void OnDestroy()
        {
            interactionAction.performed -= OnInteractionActionPerformed;
            dashAction.performed -= OnDashActionPerformed;
            runAction.performed -= OnRunActionPerformed;
            runAction.canceled -= OnRunActionCanceled;
            useAction.performed -= OnUseActionPerformed;
            throwAction.performed -= OnThrowActionPerformed;
            bagAction.performed -= OnBagActionPerformed;
            pickAction.performed -= OnPickActionPerformed;
            mouseWheelAction.performed -= OnMouseWheelActionPerformed;
            gamePauseAction.performed -= OnGamePauseActionPerformed;
        }
        
        public Vector2 MoveDir()
        {
            Vector2 inputDir = moveAction.ReadValue<Vector2>();
            return inputDir.normalized;
        }
        
        public float MouseWheelValue()
        {
            float val = mouseWheelAction.ReadValue<float>();
            return val;
        }

        public virtual bool GetDashDown() => dashAction.WasPressedThisFrame();
        public virtual bool GetIfRun() => runAction.IsPressed();
        public virtual bool GetRunUp() => runAction.WasReleasedThisFrame();
        public virtual bool GetUseDown() => useAction.WasPerformedThisFrame();
        public virtual bool GetThrowDown() => throwAction.WasPerformedThisFrame();
        public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();
        public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();
        public virtual bool GetPickDown() => pickAction.WasPerformedThisFrame();
        public virtual bool GetGamePauseDown() => gamePauseAction.WasPerformedThisFrame();
        //ֻҪ���������һ���ƶ����������ǰ���һ������
        public virtual bool GetStruggleDown() => moveAction.WasPressedThisFrame();

        #region Input Action Callbacks

        private void OnInteractionActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnInteractionPress?.Invoke();
        }

        private void OnDashActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnDashPress?.Invoke();
        }

        private void OnRunActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnRunPress?.Invoke();
        }

        private void OnRunActionCanceled(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnRunRelease?.Invoke();
        }

        private void OnUseActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnUsePress?.Invoke();
        }

        private void OnThrowActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnThrowPress?.Invoke();
        }

        private void OnBagActionPerformed(InputAction.CallbackContext context)
        {
            OnBagPress?.Invoke();
        }

        private void OnPickActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnPickPress?.Invoke();
        }

        private void OnMouseWheelActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnMouseWheelPress?.Invoke();
        }

        private void OnGamePauseActionPerformed(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.IsGamePaused) return;
            OnGamePause?.Invoke();
        }

        #endregion

        #region Utility Methods

        public string GetSettingKey(InputActionType actionType, int controlTypeIndex)
        {
            if (inputActionAsset == null)
            {
                Debug.LogError("InputActionAsset is not assigned");
                return string.Empty;
            }

            var action = inputActionAsset.FindAction(actionType.ToString());
            if (action == null)
            {
                Debug.LogError($"Could not find action '{actionType}' in input actions");
                return string.Empty;
            }

            return action.bindings[controlTypeIndex].ToDisplayString();
        }

        #endregion
    }
}