using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.InputSystem.InputBinding;

namespace KidGame.Core
{
    public class InputSettings : MonoBehaviour
    {
        private PlayerInput playerInput;
        private InputActionAsset inputActionAsset;
        private ControlMap currentControlMap;

        private InputAction interactionAction;
        private InputAction moveAction;
        private InputAction dashAction;
        private InputAction runAction;
        private InputAction useAction;
        private InputAction bagAction;
        private InputAction pickAction;
        private InputAction mouseWheelAction;
        private InputAction gamePauseAction;
        
        private InputAction UI_bagAction;
        private InputAction UI_interactionAction;
        private ClickType _m_EclickType = ClickType.Null;
        private enum ClickType
        { 
            LongPress,
            ShortPress,
            Null,
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            inputActionAsset = playerInput.actions;
            currentControlMap = ControlMap.GameMap;
            playerInput.currentActionMap = inputActionAsset.actionMaps[(int)currentControlMap];
            Init();
        }
        private void OnDestroy()
        {
            Discard();
        }
        private void Init()
        {
            //gamemap
            moveAction = inputActionAsset.FindAction("Move");
            interactionAction = inputActionAsset.FindAction("Interaction");
            dashAction = inputActionAsset.FindAction("Dash");
            runAction = inputActionAsset.FindAction("Run");
            useAction = inputActionAsset.FindAction("Use");
            bagAction = inputActionAsset.FindAction("Bag");
            pickAction = inputActionAsset.FindAction("Pick");
            mouseWheelAction = inputActionAsset.FindAction("MouseWheel");
            gamePauseAction = inputActionAsset.FindAction("GamePause");

            //uimap
            UI_bagAction = inputActionAsset.FindAction("UI_Bag");
            UI_interactionAction = inputActionAsset.FindAction("UI_Interaction");
            
            
            interactionAction.performed += OnInteractionActionPerformed;

            UI_interactionAction.performed += OnInteractionActionPerformed;
            UI_bagAction.performed += OnUIBagActionPerformed;

            dashAction.performed += OnDashActionPerformed;
            runAction.performed += OnRunActionPerformed;
            runAction.canceled += OnRunActionCanceled;
            useAction.performed += OnUseActionPerformed;
            useAction.canceled += OnUseActionCancled;

            bagAction.performed += OnBagActionPerformed;
            pickAction.performed += OnPickActionPerformed;
            mouseWheelAction.performed += OnMouseWheelActionPerformed;
            gamePauseAction.performed += OnGamePauseActionPerformed;

            this.OnUpdate(CheckMouseWheelValueChange);

            MsgCenter.RegisterMsg(MsgConst.ON_CONTROL_MAP_CHG, OnInputMapChg);
        }
        private void Discard()
        {
             
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

            UI_interactionAction.performed -= OnInteractionActionPerformed;
            UI_bagAction.performed -= OnUIBagActionPerformed;
            
            this.RemoveUpdate(CheckMouseWheelValueChange);
            MsgCenter.UnregisterMsg(MsgConst.ON_CONTROL_MAP_CHG, OnInputMapChg);


        }


        //检测滚轮值变化的核心方法
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
        //只要按了任意的一个移动键，就算是按了一次挣扎
        public virtual bool GetStruggleDown() => moveAction.WasPressedThisFrame();

        #region Input Action Callbacks

        private void OnInteractionActionPerformed(InputAction.CallbackContext context)
        {
            if (currentControlMap == ControlMap.GameMap)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_INTERACTION_PRESS);
            }
            else
            {
                MsgCenter.SendMsgAct(MsgConst.ON_UI_INTERACTION_PRESS);
            }
            
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
            // 只有当长按交互被取消且动作确实执行过
            // Unity系统没法分辨是否是长按取消还是短按 得判断这个动作是否真的被Performed过
            // 改了一下发现不行 这里就用枚举判断了
            if (context.interaction is PressInteraction && _m_EclickType == ClickType.ShortPress)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_USE_SHORT_PRESS_RELEASE);
            }
            else if (context.interaction is HoldInteraction && _m_EclickType == ClickType.LongPress)
            {
                MsgCenter.SendMsgAct(MsgConst.ON_USE_LONG_PRESS_RELEASE);
            }
            _m_EclickType = ClickType.Null;
        }
        private void OnUseActionPerformed(InputAction.CallbackContext context)
        {
            //如果是点击
            if(context.interaction is PressInteraction)
            {
                _m_EclickType = ClickType.ShortPress;
                MsgCenter.SendMsgAct(MsgConst.ON_USE_PRESS);
            }
            //如果是长按
            if (context.interaction is HoldInteraction)
            {
                _m_EclickType = ClickType.LongPress;
                MsgCenter.SendMsgAct(MsgConst.ON_USE_LONG_PRESS);
            }
                
        }

        private void OnBagActionPerformed(InputAction.CallbackContext context)
        {
            MsgCenter.SendMsgAct(MsgConst.ON_BAG_PRESS);
        }
        
        private void OnUIBagActionPerformed(InputAction.CallbackContext context)
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
            //只展示键位 其他信息都不展示
            return action.bindings[controlTypeIndex].ToDisplayString(DisplayStringOptions.DontIncludeInteractions);
        }
        
        public Vector3 CameraRelativeMoveDir(Transform cam)
        {
            Vector2 in2 = MoveDir();
            if (in2.sqrMagnitude < 0.0001f) return Vector3.zero;
            
            Vector3 fwd = cam.forward; fwd.y = 0f; fwd.Normalize();
            Vector3 right = cam.right; right.y = 0f; right.Normalize();
            
            Vector3 move = fwd * in2.y + right * in2.x;
            return move.normalized;
        }
        
        #endregion

        #region RegisterCallbacks
        private void OnInputMapChg(object[] objs)
        {
            if (objs == null || objs.Length == 0) return;
            currentControlMap = (ControlMap)objs[0];
            playerInput.currentActionMap = inputActionAsset.actionMaps[(int)currentControlMap];
            Discard();
            Init();
        }

        #endregion
    }
}