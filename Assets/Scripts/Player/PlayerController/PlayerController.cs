using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public partial class PlayerController : Singleton<PlayerController>, IStateMachineOwner, IDamageable, ISoundable
    {
        private InputSettings inputSettings;
        public InputSettings InputSettings => inputSettings;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        public float ReceiveSoundRange => 0;
        public GameObject SoundGameObject => gameObject;

        public PlayerAnimator PlayerAnimator;
        private BuffHandler playerBuffHandler;
        private StateMachine stateMachine;

        public Transform PlaceTrapPoint;
        public Transform SpawnAndUseThrowWeaponPoint;
        public Transform SpawnAndUseOnHandWeaponPoint;

        private PlayerInfo playerInfo;
        public PlayerInfo PlayerInfo => playerInfo;

        protected override void Awake()
        {
            base.Awake();
            inputSettings = GetComponent<InputSettings>();
            rb = GetComponent<Rigidbody>();
        }

        public void Init()
        {
            playerInfo = GameModel.Instance.PlayerInfo;

            //初始化状态机
            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //初始化buff处理器
            playerBuffHandler = new BuffHandler();
            playerBuffHandler.Init();
            //初始化为Idle状态
            ChangeState(PlayerState.Idle);
            LogicSoundManager.Instance.RegSoundable(this);
            //注册一些事件
            RegActions();
        }

        public void Discard()
        {
            stateMachine.ObjectPushPool();
            playerBuffHandler.Discard();
            LogicSoundManager.Instance.UnregSoundable(this);
            UnregActions();
        }

        public void UpdateStamina()
        {
            if (playerInfo.IsRecovering)
            {
                // 恢复体力
                playerInfo.CurrentStamina += playerInfo.StaminaRecoverPerSecond * Time.deltaTime;
                playerInfo.CurrentStamina = Mathf.Clamp(playerInfo.CurrentStamina, 0, playerInfo.MaxStamina);
                MsgCenter.SendMsg(MsgConst.ON_STAMINA_CHG, playerInfo.CurrentStamina / playerInfo.MaxStamina);
                
                // 检查是否恢复足够
                if (playerInfo.CurrentStamina >= playerInfo.MaxStamina * playerInfo.RecoverThreshold)
                {
                    playerInfo.IsExhausted = false;
                }

                if (playerInfo.CurrentStamina >= playerInfo.MaxStamina)
                {
                    playerInfo.CurrentStamina = playerInfo.MaxStamina;
                    playerInfo.IsRecovering = false;
                }
            }
        }

        public string GetSettingKey(InputActionType actionType, ControlType controlType)
        {
            return inputSettings.GetSettingKey(actionType, (int)controlType);
        }
    }
}