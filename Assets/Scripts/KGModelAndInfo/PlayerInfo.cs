using KidGame.Core;
using KidGame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.core
{
    public class PlayerInfo
    {
        #region 私有字段

        private PlayerBaseData PlayerBaseData;

        #region 玩家挣扎
        private float struggleDemand = 1f;
        private float currentStruggle = 0f;
        #endregion


        #region 手持武器相关
        private WeaponData currentWeaponData = null;
        #endregion


        #region 玩家基础信息

        private Quaternion playerRotate;

        private bool isInvulnerable = false;

        private int currentHealth;

        private bool isExhausted = false;

        private float currentStamina;

        private bool isRecovering = false;


        #endregion

        #region 玩家交互

        //key:可交互 value:和玩家距离
        private Dictionary<IInteractive, float> interactiveDict = new Dictionary<IInteractive, float>();
        //key:可回收 value:和玩家距离
        private Dictionary<IPickable, float> pickableDict = new Dictionary<IPickable, float>();

        private Vector3 mouseWorldPos;
        private Vector3 playerBottomPos;
        private Vector3 rotateDir;

        #endregion 玩家交互层

        #endregion 私有字段层

        #region 属性字段
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
        public float struggleAmountOneTime => PlayerBaseData.StruggleAmountOneTime;

        // 挣扎后的无敌时间
        public float struggleInvulnerabilityDuration => PlayerBaseData.StruggleInvulnerabilityDuration;

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
        public int MaxHealth => PlayerBaseData.Hp;
        public float MaxStamina => PlayerBaseData.Sp;

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

        #endregion

    }

}
