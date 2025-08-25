using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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
        private InputAction bagAction;
        private InputAction pickAction;
        private InputAction mouseWheelAction;
        private InputAction gamePauseAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            inputActionAsset = playerInput.actions;
            
            moveAction = inputActionAsset.FindAction("Move");
            interactionAction = inputActionAsset.FindAction("Interaction");
            dashAction = inputActionAsset.FindAction("Dash");
            runAction = inputActionAsset.FindAction("Run");
            useAction = inputActionAsset.FindAction("Use");
            bagAction = inputActionAsset.FindAction("Bag");
            pickAction = inputActionAsset.FindAction("Pick");
            mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
            gamePauseAction = inputActionAsset.FindAction("GamePause");

            
            //������������óɰ��¿�ʼ��Ӧ ��ֹ��������e��һ��ʰȡ��e�ֲ���
            interactionAction.performed += OnInteractActionPerformedWithoutTime;
            interactionAction.performed += OnInteractionActionPerformed;
            
            
            dashAction.performed += OnDashActionPerformed;
            runAction.performed += OnRunActionPerformed;
            runAction.canceled += OnRunActionCanceled;
            useAction.performed += OnUseActionPerformed;
            useAction.canceled += OnUseActionCancled;

            bagAction.performed+= OnBagActionPerformed;
            pickAction.performed += OnPickActionPerformed;
            mouseWheelAction.performed += OnMouseWheelActionPerformed;
            gamePauseAction.performed += OnGamePauseActionPerformed;

            this.OnUpdate(CheckMouseWheelValueChange);
        }

        private void OnDestroy()
        {
            interactionAction.performed -= OnInteractActionPerformedWithoutTime;    
            interactionAction.performed -= OnInteractionActionPerformed;
            dashAction.performed -= OnDashActionPerformed;
            runAction.performed -= OnRunActionPerformed;
            runAction.canceled -= OnRunActionCanceled;
            useAction.performed -= OnUseActionPerformed;
            useAction.canceled -= OnUseActionCancled;
            bagAction.performed -= OnBagActionPerformed;
            pickAction.performed -= OnPickActionPerformed;
            mouseWheelAction.performed -= OnMouseWheelActionPerformed;
            gamePauseAction.performed -= OnGamePauseActionPerformed;

            this.RemoveUpdate(CheckMouseWheelValueChange);

        }


        //������ֵ�仯�ĺ��ķ���
        private void CheckMouseWheelValueChange()
        {
            if (MouseWheelValue() == 0) return;
            float currentValue = MouseWheelValue();
            MsgCenter.SendMsg(MsgConst.ON_MOUSEWHEEL_VALUE_CHG, currentValue);
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
        public virtual bool GetBagDown() => bagAction.WasPerformedThisFrame();
        public virtual bool GetInteractDown() => interactionAction.WasPerformedThisFrame();
        public virtual bool GetPickDown() => pickAction.WasPerformedThisFrame();
        public virtual bool GetGamePauseDown() => gamePauseAction.WasPerformedThisFrame();
        //ֻҪ���������һ���ƶ����������ǰ���һ������
        public virtual bool GetStruggleDown() => moveAction.WasPressedThisFrame();

        #region Input Action Callbacks

        private void OnInteractionActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_INTERACTION_PRESS);
        }
        //����ʱ�����ŵĻ����¼�
        private void OnInteractActionPerformedWithoutTime(InputAction.CallbackContext context)
        {
            // ֻ�ڰ�ѹ���ʱ����һ�Σ���Ҫ���ǰ�����õ�"press"�������ͣ�
            MsgCenter.SendMsgAct(MsgConst.ON_INTERACTION_PRESS_WITHOUT_TIME);
        }

        private void OnDashActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_DASH_PRESS);
        }

        private void OnRunActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_RUN_PRESS);
        }

        private void OnRunActionCanceled(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_RUN_RELEASE);
        }

        private void OnUseActionCancled(InputAction.CallbackContext context)
        {
            if(context.interaction is HoldInteraction)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_USE_LONG_PRESS_RELEASE);
            }
        }
        private void OnUseActionPerformed(InputAction.CallbackContext context)
        {
            //����ǵ��
            if(context.interaction is PressInteraction)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_USE_PRESS);
            }
            //����ǳ���
            if (context.interaction is HoldInteraction)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_USE_LONG_PRESS);
            }
                
        }

        private void OnBagActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_BAG_PRESS);
        }

        private void OnPickActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_PICK_PRESS);
        }

        private void OnMouseWheelActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_MOUSEWHEEL_PRESS);
        }

        private void OnGamePauseActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_GAMEPAUSE_PRESS);
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