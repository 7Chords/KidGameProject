﻿using System.Collections;
using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System;
using Unity.VisualScripting;
using KidGame.UI;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : Singleton<PlayerController>, IStateMachineOwner, IDamageable,ISoundable
    {
        #region 玩家受伤

        [SerializeField] private List<string> randomDamgeSfxList;

        public List<string> RandomDamgeSfxList
        {
            get => randomDamgeSfxList;
            set { randomDamgeSfxList = value; }
        }

        [SerializeField] private ParticleSystem damagePartical;

        #endregion

        #region 组件
        
        public ParticleSystem DamagePartical
        {
            get => damagePartical;
            set { damagePartical = value; }
        }
        
        private InputSettings inputSettings;
        public InputSettings InputSettings => inputSettings;

        private Rigidbody rb;
        public Rigidbody Rb => rb;
        
        public PlayerAnimator PlayerAnimator;
        public PlayerBaseData PlayerBaseData;

        private BuffHandler playerBuffHandler;

        public Transform PlaceTrapPoint;
        private Vector3 SpawnWeaponOffset = new Vector3(0, 1, 0.5f);
        private GameObject curPreviewGO;
        #endregion

        #region 状态机

        private StateMachine stateMachine;
        private PlayerState playerState; // 玩家的当前状态        
        #endregion

        #region 玩家挣扎
        private float struggleDemand = 1f;
        private float struggleAmountOneTime = 10f;
        private float currentStruggle = 0f;
        private float struggleInvulnerabilityDuration = 1f; // 挣扎后的无敌时间
        #endregion
        
        #region 玩家生命值

        private float currentHealth;
        private bool isInvulnerable = false;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => PlayerBaseData.Hp;

        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        #endregion
        
        #region 玩家体力值

        private float currentStamina = 100f;
        private float maxStamina = 100f;
        private bool isExhausted = false;
        private bool isRecovering = false;
        
        // 体力恢复速率
        private float staminaRecoverRate = 2f;
        private float recoverThreshold = 0.25f;
        
        public event Action<float> OnStaminaChanged;

        #endregion

        #region 玩家交互

        //key:可交互 value:和玩家距离
        private Dictionary<IInteractive, float> interactiveDict;
        //key:可回收 value:和玩家距离
        private Dictionary<IPickable, float> pickableDict;

        public event Action<float> OnMouseWheelValueChanged;
        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            inputSettings = GetComponent<InputSettings>();

            rb = GetComponent<Rigidbody>();
            currentHealth = PlayerBaseData.Hp;
            currentStamina = maxStamina;
        }
        

        public void Init()
        {
            stateMachine = PoolManager.Instance.GetObject<StateMachine>();
            stateMachine.Init(this);
            //初始化为Idle状态
            ChangeState(PlayerState.Idle);
            //初始化buff处理器
            playerBuffHandler = new BuffHandler();
            playerBuffHandler.Init();

            //注册一些事件
            RegActions();

            //初始化字典
            interactiveDict = new Dictionary<IInteractive, float>();
            pickableDict = new Dictionary<IPickable, float>();
        }

        public void Discard()
        {
            stateMachine.ObjectPushPool();
            playerBuffHandler.Discard();
            UnregActions();
        }

        #endregion

        #region 手持武器相关
        private WeaponData currentWeaponData = null;
        private GameObject currentWeapon = null;
        #endregion

        #region 事件相关

        /// <summary>
        /// 注册事件们
        /// </summary>
        private void RegActions()
        {
            inputSettings.OnInteractionPress += PlayerInteraction;
            inputSettings.OnPickPress += PlayerPick;
            inputSettings.OnUsePress += TryPlaceTrap;
            inputSettings.OnBagPress += ControlBag;
            inputSettings.OnGamePause += GamePause;
            inputSettings.OnMouseWheelValueChanged += SwitchSelectItem;

            PlayerBag.Instance.OnSelectItemAction += OnItemSelected;
        }


        /// <summary>
        /// 反注册事件们
        /// </summary>
        private void UnregActions()
        {
            inputSettings.OnInteractionPress -= PlayerInteraction;
            inputSettings.OnPickPress -= PlayerPick;
            inputSettings.OnUsePress -= TryPlaceTrap;
            inputSettings.OnBagPress -= ControlBag;
            inputSettings.OnGamePause -= GamePause;
            inputSettings.OnMouseWheelValueChanged -= SwitchSelectItem;

            PlayerBag.Instance.OnSelectItemAction -= OnItemSelected;

        }

        #endregion

        #region 功能
        

        public bool IsPlayerState(PlayerState state)
        {
            return state == playerState;
        }

        private void GamePause()
        {
            Signals.Get<OpenPauseSignal>().Dispatch();
        }

        private void ControlBag()
        {
            Signals.Get<ControlBagSignal>().Dispatch();
        }
        /// <summary>
        /// 玩家旋转 TODO:优化？
        /// </summary>
        public void Rotate(Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
        public void ChangeState(PlayerState playerState, bool reCurrstate = false)
        {
            // 如果处于体力耗尽状态只能进入Idle状态
            if (isExhausted && (playerState == PlayerState.Move || playerState == PlayerState.Dash))
            {
                playerState = PlayerState.Idle;
            }

            this.playerState = playerState;
            switch (playerState)
            {
                case PlayerState.Idle:
                    stateMachine.ChangeState<PlayerIdleState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Move:
                    stateMachine.ChangeState<PlayerMoveState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Dash:
                    stateMachine.ChangeState<PlayerDashState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Use:
                    stateMachine.ChangeState<PlayerUseState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Dead:
                    stateMachine.ChangeState<PlayerDeadState>((int)playerState, reCurrstate);
                    break;
                case PlayerState.Struggle:
                    stateMachine.ChangeState<PlayerStruggleState>((int)playerState, reCurrstate);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            PlayerAnimator.PlayAnimation(animationName);
        }

        /// <summary>
        /// 玩家交互
        /// </summary>
        public void PlayerInteraction()
        {
            if (interactiveDict == null || interactiveDict.Count == 0) return;
            GetClosestInteractive()?.InteractPositive(gameObject);
        }

        public void PlayerPick()
        {
            if (pickableDict == null || pickableDict.Count == 0) return;
            GetClosestPickable()?.Pick();
        }
        /// <summary>
        /// 尝试放陷阱
        /// </summary>
        

        public void TryPlaceTrap()
        {
            if (playerState != PlayerState.Idle)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("当前状态不可布置陷阱", gameObject));
                return;
            }
            ChangeState(PlayerState.Use);
        }

        /// <summary>
        /// 添加到可交互列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddInteractiveToList(IInteractive interactive, float distance)
        {
            if (interactiveDict == null) return;
            if (interactiveDict.ContainsKey(interactive)) return;
            interactiveDict.Add(interactive, distance);
        }

        /// <summary>
        /// 从可交互列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveInteractiveFromList(IInteractive interactive)
        {
            if (interactiveDict == null) return;
            if (!interactiveDict.ContainsKey(interactive)) return;
            interactiveDict.Remove(interactive);
        }

        /// <summary>
        /// 获得最近的可交互者
        /// </summary>
        private IInteractive GetClosestInteractive()
        {
            float min = 999;
            IInteractive closestInteractive = null;
            foreach (var pair in interactiveDict)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    closestInteractive = pair.Key;
                }
            }

            return closestInteractive;
        }

        /// <summary>
        /// 添加到可回收列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddPickableToList(IPickable pickable, float distance)
        {
            if (pickableDict == null) return;
            if (pickableDict.ContainsKey(pickable)) return;
            pickableDict.Add(pickable, distance);
        }

        /// <summary>
        /// 从可回收列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemovePickableFromList(IPickable pickable)
        {
            if (pickableDict == null) return;
            if (!pickableDict.ContainsKey(pickable)) return;
            pickableDict.Remove(pickable);
        }

        /// <summary>
        /// 获得最近的可拾取者
        /// </summary>
        public IPickable GetClosestPickable()
        {
            float min = 999;
            IPickable closestIPickable = null;
            foreach (var pair in pickableDict)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    closestIPickable = pair.Key;
                }
            }

            return closestIPickable;
        }


        /// <summary>
        /// 制造声音
        /// </summary>
        public void ProduceSound(float range)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, range);
            if (colls.Length == 0) return;
            ISoundable soundable = null;
            foreach(var coll in colls)
            {
                soundable = coll.GetComponent<ISoundable>();
                if (soundable == null) continue;
                soundable.ReceiveSound(gameObject);
            }
        }

        /// <summary>
        /// 接收声音
        /// </summary>
        /// <param name="creator"></param>
        public void ReceiveSound(GameObject creator) { }


        /// <summary>
        /// 切换选择的物品
        /// </summary>
        /// <param name="scrollValue">鼠标滚轮值</param>
        public void SwitchSelectItem(float scrollValue)
        {
            OnMouseWheelValueChanged?.Invoke(scrollValue);
        }

        private void OnItemSelected(ISlotInfo slotInfo)
        {
            if(slotInfo == null)
            {
                DestoryCurrentTrapPreview();
                DiscardWeapon();
                return;
            }
            if(slotInfo.ItemData is TrapData trapData)
            {
                SpawnSelectTrapPreview(trapData);
            }
            else if(slotInfo.ItemData is WeaponData weaponData)
            {
                // to do: 不要每次都生成一个武器
                if (currentWeaponData != null && weaponData.id == currentWeaponData.id) return;
                // 如果不是重复的 销毁现在的 取得新的
                DiscardWeapon();
                currentWeapon = SpawnWeaponOnHand(
                    weaponData,
                    this.transform.position,
                    this.transform.rotation
                    );
                
            }
        }

        public void DiscardWeapon()
        {
            if(currentWeapon!=null)
            {
                Destroy(currentWeapon);
                currentWeapon = null;
            }
        }

        /// <summary>
        /// 生成预览的陷阱
        /// </summary>
        /// <param name="obj"></param>
        private void SpawnSelectTrapPreview(TrapData trapData)
        {
            DestoryCurrentTrapPreview();
            curPreviewGO = TrapFactory.CreatePreview(trapData, PlaceTrapPoint.position, transform);
        }

        private void DestoryCurrentTrapPreview()
        {
            if (curPreviewGO) Destroy(curPreviewGO);
        }

        //生成在手上的武器 但是不执行逻辑
        public GameObject SpawnWeaponOnHand(WeaponData weaponData, Vector3 position, Quaternion rotation)
        {

            GameObject newWeapon = WeaponFactory.CreateEntity(weaponData, SpawnWeaponOffset, this.gameObject.transform);
            if (newWeapon != null)
            {
                newWeapon.transform.rotation = rotation;
            }
            return newWeapon;
        }

        #endregion

        #region 体力与生命

        /// <summary>
        /// 受伤
        /// </summary>
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (isInvulnerable) return;
    
            currentHealth -= damageInfo.damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            
            OnHealthChanged?.Invoke(currentHealth / MaxHealth);
    
            if (currentHealth <= 0)
            {
                ChangeState(PlayerState.Dead);
            }
            else
            {
                StartStruggle();
            }
        }

        private void StartStruggle()
        {
            ChangeState(PlayerState.Struggle);
            isInvulnerable = true;
            currentStruggle = 0f;
        }

        public void Struggle()
        {
           currentStruggle += Time.deltaTime * struggleAmountOneTime;
           Debug.Log(currentStruggle);
           
            // 挣扎完成
            if (currentStruggle >= struggleDemand)
            {
                EndStruggle();
            }
        }

        private void EndStruggle()
        {
            ChangeState(PlayerState.Idle);
            StartCoroutine(EndInvulnerabilityAfterTime(struggleInvulnerabilityDuration));
        }

        private IEnumerator EndInvulnerabilityAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            isInvulnerable = false;
        }
        
        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="healAmount">恢复值</param>
        public void Heal(float healAmount)
        {
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
            OnHealthChanged?.Invoke(currentHealth / MaxHealth);
        }

        public void Dead()
        {
            //TODO:临时测试
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
        
        public void UpdateStamina()
        {
            if (isRecovering)
            {
                // 恢复体力
                currentStamina += staminaRecoverRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
                OnStaminaChanged?.Invoke(currentStamina / maxStamina);
                
                // 检查是否恢复足够
                if (currentStamina >= maxStamina * recoverThreshold)
                {
                    isExhausted = false;
                }

                if (currentStamina >= maxStamina)
                {
                    currentHealth = maxStamina;
                    isRecovering = false;
                }
            }
        }
        
        public bool ConsumeStamina(float amount)
        {
            if (isExhausted) return false;
            
            currentStamina -= amount;
            
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                return false;
            }
            OnStaminaChanged?.Invoke(currentStamina / maxStamina);
            if (currentStamina <= maxStamina * recoverThreshold)
            {
                isExhausted = true;
            }

            if (currentStamina <= maxStamina)
            {
                isRecovering = true;
            }
            
            return true;
        }

        #endregion

        public bool GetCanPlaceTrap()
        {
            if (curPreviewGO == null) return false;
            return curPreviewGO.GetComponentInParent<TrapBase>().CanPlaceTrap;
        }
    }
}