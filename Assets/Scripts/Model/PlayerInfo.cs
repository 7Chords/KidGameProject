using KidGame.Core.Data;
using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerInfo
    {
        #region --私有字段--

        private PlayerBaseData playerBaseData;

        private float struggleDemand;

        private float currentStruggle;

        private WeaponData currentWeaponData;

        private Quaternion playerRotate;

        private bool isInvulnerable;

        private int currentHealth;

        private bool isExhausted;

        private float currentStamina;

        private bool isRecovering;

        private bool isHide;

        private PlayerState playerState;

        private GameObject curPreviewTrapGO;

        private GameObject curWeaponGO;

        //key:可交互 value:和玩家距离
        private Dictionary<IInteractive, float> interactiveDict;
        //key:可回收 value:和玩家距离
        private Dictionary<IPickable, float> pickableDict;

        private Vector3 mouseWorldPos;

        private Vector3 playerBottomPos;

        private Vector3 rotateDir;

        #endregion



        #region --属性字段--
        // 筋疲力尽
        public bool IsExhausted
        {
            get => isExhausted;
            set => isExhausted = value; 
        }

        // 解脱束缚阈值
        public float StruggleDemand
        {
            get => struggleDemand;
            set => struggleDemand = value;
        }

        // 现在的挣扎进度
        public float CurrentStruggle
        {
            get => currentStruggle;
            set => currentStruggle = value;
        }
        // 单次挣扎增加解脱值数 struggleInvulnerabilityDuration
        public float StruggleAmountOneTime => playerBaseData.StruggleAmountOneTime;

        // 挣扎后的无敌时间
        public float StruggleInvulnerabilityDuration => playerBaseData.StruggleInvulnerabilityDuration;

        public float StaminaRecoverPerSecond => playerBaseData.StaminaRecoverPerSecond;

        public float RecoverThreshold => playerBaseData.RecoverThreshold;
        public float DashStaminaOneTime => playerBaseData.DashStaminaOneTime;
        public float RunStaminaPerSecond => playerBaseData.RunStaminaPerSecond;

        public float RunSpeed => playerBaseData.RunSpeed;

        public float WalkSpeed => playerBaseData.WalkSpeed;

        // 手持武器相关属性
        public WeaponData CurrentWeaponData
        {
            get => currentWeaponData;
            set => currentWeaponData = value;
        }

        // 玩家基础信息属性
        public Quaternion PlayerRotate
        {
            get => playerRotate;
            set => playerRotate = value;
        }

        public bool IsInvulnerable
        {
            get => isInvulnerable;
            set => isInvulnerable = value;
        }

        public int CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = value;
        }
        public int MaxHealth => playerBaseData.Hp;
        public float MaxStamina => playerBaseData.Sp;

        public float CurrentStamina
        {
            get => currentStamina;
            set => currentStamina = value;
        }

        public bool IsRecovering
        {
            get => isRecovering;
            set => isRecovering = value;
        }

        public Vector3 MouseWorldPos
        {
            get => mouseWorldPos;
            set => mouseWorldPos = value;
        }
        public bool IsHide
        {
            get => isHide;
            set => isHide = value;
        }

        public Vector3 PlayerBottomPos
        {
            get => playerBottomPos;
            set => playerBottomPos = value;
        }

        public Vector3 RotateDir
        {
            get => rotateDir;
            set => rotateDir = value;
        }

        public Dictionary<IInteractive, float> InteractiveDict
        {
            get => interactiveDict;
            set => interactiveDict = value;
        }

        public Dictionary<IPickable, float> PickableDict
        {
            get => pickableDict;
            set => pickableDict = value;
        }

        public PlayerState PlayerState
        {
            get => playerState;
            set => playerState = value;
        }
        public GameObject CurPreviewTrapGO
        {
            get => curPreviewTrapGO;
            set => curPreviewTrapGO = value;
        }
        public GameObject CurWeaponGO
        {
            get => curWeaponGO;
            set => curWeaponGO = value;
        }
        #endregion




        #region --数据方法--
        public void Init(PlayerBaseData playerData)
        {
            playerBaseData = playerData;

            // 挣扎系统相关
            struggleDemand = 1f;
            currentStruggle = 0f;

            // 武器数据
            currentWeaponData = null;

            // 玩家旋转信息
            playerRotate = Quaternion.identity;

            // 无敌状态
            isInvulnerable = false;

            // 生命值（如果PlayerBaseData已初始化，使用其最大值）
            currentHealth = playerBaseData != null ? playerBaseData.Hp : 0;

            // 疲劳状态
            isExhausted = false;

            // 耐力值（如果PlayerBaseData已初始化，使用其最大值）
            currentStamina = playerBaseData != null ? playerBaseData.Sp : 0f;

            // 恢复状态
            isRecovering = false;

            // 躲藏状态
            isHide = false;

            // 玩家状态
            playerState = PlayerState.Idle;

            // 预览陷阱游戏对象
            curPreviewTrapGO = null;

            // 交互字典初始化
            interactiveDict = new Dictionary<IInteractive, float>();
            pickableDict = new Dictionary<IPickable, float>();

            // 位置和方向信息
            mouseWorldPos = Vector3.zero;
            playerBottomPos = Vector3.zero;
            rotateDir = Vector3.zero;
        }


        #endregion

    }

}
