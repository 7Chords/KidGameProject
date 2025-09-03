using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : Singleton<PlayerController>, IStateMachineOwner, IDamageable, ISoundable
    {

        private InputSettings inputSettings;
        public InputSettings InputSettings => inputSettings;

        private Rigidbody rb;
        public Rigidbody Rb => rb;

        public PlayerAnimator PlayerAnimator;
        private BuffHandler playerBuffHandler;
        private StateMachine stateMachine;

        public Transform PlaceTrapPoint;
        public Transform SpawnAndUseThrowWeaponPoint;
        public Transform SpawnAndUseOnHandWeaponPoint;

        private PlayerInfo playerInfo;


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

            //注册一些事件
            RegActions();

        }

        public void Discard()
        {
            stateMachine.ObjectPushPool();
            playerBuffHandler.Discard();
            UnregActions();
        }


        #region 事件相关

        /// <summary>
        /// 注册事件们
        /// </summary>
        private void RegActions()
        {
            MsgCenter.RegisterMsgAct(MsgConst.ON_INTERACTION_PRESS, PlayerInteraction);
            MsgCenter.RegisterMsgAct(MsgConst.ON_PICK_PRESS, PlayerPick);
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_PRESS, PlayerUseItem);
            MsgCenter.RegisterMsgAct(MsgConst.ON_BAG_PRESS, ControlBag);
            MsgCenter.RegisterMsgAct(MsgConst.ON_GAMEPAUSE_PRESS, GamePause);
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_LONG_PRESS, TryUseWeaponUseLongPress);
            MsgCenter.RegisterMsg(MsgConst.ON_SELECT_ITEM, OnItemSelected);
            MsgCenter.RegisterMsg(MsgConst.ON_PLAYER_UNDER_TABLE_CHG, OnPlayerUnderTableChg);
        }


        /// <summary>
        /// 反注册事件们
        /// </summary>
        private void UnregActions()
        {
            MsgCenter.UnregisterMsgAct(MsgConst.ON_INTERACTION_PRESS, PlayerInteraction);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_PICK_PRESS, PlayerPick);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_PRESS, PlayerUseItem);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_BAG_PRESS, ControlBag);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_GAMEPAUSE_PRESS, GamePause);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_LONG_PRESS, TryUseWeaponUseLongPress);
            MsgCenter.UnregisterMsg(MsgConst.ON_SELECT_ITEM, OnItemSelected);
            MsgCenter.UnregisterMsg(MsgConst.ON_PLAYER_UNDER_TABLE_CHG, OnPlayerUnderTableChg);

        }

        #endregion

        #region 功能


        public bool IsPlayerState(PlayerState state)
        {
            return state == playerInfo.PlayerState;
        }

        private void GamePause()
        {
            Signals.Get<OpenPauseSignal>().Dispatch();
        }

        private void ControlBag()
        {
            Signals.Get<ControlBackpackPanelSignal>().Dispatch();
        }
        /// <summary>
        /// 玩家旋转
        /// </summary>
        public void Rotate()
        {
            //将鼠标屏幕坐标转换为世界坐标
            playerInfo.MouseWorldPos = MouseRaycaster.Instance.GetMousePosi();
            //忽略Y轴差异
            playerInfo.PlayerBottomPos = new Vector3(transform.position.x, playerInfo.MouseWorldPos.y, transform.position.z);
            //计算方向向量
            playerInfo.RotateDir = (playerInfo.MouseWorldPos - playerInfo.PlayerBottomPos).normalized;
            transform.rotation = Quaternion.LookRotation(playerInfo.RotateDir);
        }
        public void ChangeState(PlayerState playerState, bool reCurrstate = false)
        {
            // 如果处于体力耗尽状态只能进入Idle状态
            if (playerInfo.IsExhausted && (playerState == PlayerState.Move || playerState == PlayerState.Dash))
            {
                playerState = PlayerState.Idle;
            }

            playerInfo.PlayerState = playerState;
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
            if (playerInfo.InteractiveDict == null || playerInfo.InteractiveDict.Count == 0) return;
            GetClosestInteractive()?.InteractPositive(gameObject);
        }

        public void PlayerPick()
        {
            if (playerInfo.PickableDict == null || playerInfo.PickableDict.Count == 0) return;
            GetClosestPickable()?.Pick();
        }


        /// <summary>
        /// 玩家使用物品
        /// </summary>
        public void PlayerUseItem()
        {
            if (!CanPlayerUseItem()) return;

            ISlotInfo currentUseItem = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (currentUseItem == null) return;
            UseItemType itemType = currentUseItem.ItemData.UseItemType;
            //允许变走边使用武器
            if (playerInfo.PlayerState != PlayerState.Idle && itemType != UseItemType.weapon)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("当前状态不可布置陷阱", transform.position));
                return;
            }
            if (currentUseItem is WeaponSlotInfo weaponItem)
            {
                //不是短按类型的武器 我直接拦截
                if (weaponItem.weaponData.longOrShortPress == 0) return;
            }
            ChangeState(PlayerState.Use);
        }

        private bool CanPlayerUseItem()
        {
            if(playerInfo.IsPlayerUnderDesk)
            {
                return false;
            }
            return true;
        }

        public void TryUseWeaponUseLongPress()
        {
            //Logic
        }
        
        /// <summary>
        /// 添加到可交互列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddInteractiveToList(IInteractive interactive, float distance)
        {
            if (playerInfo.InteractiveDict == null) return;
            if (playerInfo.InteractiveDict.ContainsKey(interactive)) return;
            playerInfo.InteractiveDict.Add(interactive, distance);
        }

        /// <summary>
        /// 从可交互列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveInteractiveFromList(IInteractive interactive)
        {
            if (playerInfo.InteractiveDict == null) return;
            if (!playerInfo.InteractiveDict.ContainsKey(interactive)) return;
            playerInfo.InteractiveDict.Remove(interactive);
        }

        /// <summary>
        /// 获得最近的可交互者
        /// </summary>
        private IInteractive GetClosestInteractive()
        {
            float min = 999;
            IInteractive closestInteractive = null;
            foreach (var pair in playerInfo.InteractiveDict)
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
            if (playerInfo.PickableDict == null) return;
            if (playerInfo.PickableDict.ContainsKey(pickable)) return;
            playerInfo.PickableDict.Add(pickable, distance);
        }

        /// <summary>
        /// 从可回收列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemovePickableFromList(IPickable pickable)
        {
            if (playerInfo.PickableDict == null) return;
            if (!playerInfo.PickableDict.ContainsKey(pickable)) return;
            playerInfo.PickableDict.Remove(pickable);
        }

        /// <summary>
        /// 获得最近的可拾取者
        /// </summary>
        private IPickable GetClosestPickable()
        {
            float min = 999;
            IPickable closestIPickable = null;
            foreach (var pair in playerInfo.PickableDict)
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
            foreach (var coll in colls)
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


        private void OnItemSelected(object[] objs)
        {
            ISlotInfo slotInfo;
            if (objs == null)
                slotInfo = null;
            else
                slotInfo = objs[0] as ISlotInfo;

            if (slotInfo == null || slotInfo.ItemData is MaterialData || slotInfo.ItemData is FoodData)
            {
                DestoryCurrentTrapPreview();
                DiscardWeaponAndWeaponData();
                return;
            }
            if(slotInfo.ItemData is TrapData trapData)
            {
                SpawnSelectTrapPreview(trapData);
            }
            else if(slotInfo.ItemData is WeaponData weaponData)
            {
                if (playerInfo.CurrentWeaponData != null && weaponData.id == playerInfo.CurrentWeaponData.id) return;
                // 消耗品  远程
                if (weaponData.useType == 0 && weaponData.weaponType == 1)
                {

                    // 如果不是重复的 销毁现在的 取得新的
                    // 不销毁 只让引用为空 销毁在逻辑里做了 否则会有一些问题
                    DiscardWeaponAndWeaponData();
                    playerInfo.CurWeaponGO = SpawnThrowWeapon(
                        weaponData,
                        this.transform.rotation
                        );
                }
                //可多次使用  近战
                else if(weaponData.useType == 1 && weaponData.weaponType == 0)
                {
                    DiscardWeaponAndWeaponData();
                    playerInfo.CurWeaponGO = SpawnOnHandWeapon(
                        weaponData,
                        SpawnAndUseOnHandWeaponPoint.rotation
                        );
                }
 
            }
            
        }

        public void SetCurrentWeaponData(WeaponData _weaponData)
        {
            playerInfo.CurrentWeaponData = _weaponData;
        }
        public  GameObject GetCurWeapon()
        {
            return playerInfo.CurWeaponGO;
        }

        public void SetWeaponAndWeaponDataReference2Null()
        {
            Debug.Log("YES");
            playerInfo.CurWeaponGO = null;
            playerInfo.CurrentWeaponData = null;
        }
        public void DiscardWeaponAndWeaponData()
        {
            if(playerInfo.CurWeaponGO != null && playerInfo.CurrentWeaponData != null)
            {
                Destroy(playerInfo.CurWeaponGO);
            }
            playerInfo.CurWeaponGO = null;
            playerInfo.CurrentWeaponData = null;
        }


        /// <summary>
        /// 生成预览的陷阱
        /// </summary>
        /// <param name="obj"></param>
        private void SpawnSelectTrapPreview(TrapData trapData)
        {
            DestoryCurrentTrapPreview();
            playerInfo.CurPreviewTrapGO = TrapFactory.CreatePreview(trapData, PlaceTrapPoint.position, transform);
        }

        private void DestoryCurrentTrapPreview()
        {
            //在放置陷阱时切换 立刻结束放置状态
            if (playerInfo.PlayerState == PlayerState.Use)
            {
                ChangeState(PlayerState.Idle);
                UIHelper.Instance.DestoryCircleProgress(GlobalValue.CIRCLE_PROGRESS_PLACE_TRAP);
            }
            if (playerInfo.CurPreviewTrapGO) Destroy(playerInfo.CurPreviewTrapGO);
        }

        //生成在手上的武器 但是不执行逻辑
        public GameObject SpawnThrowWeapon(WeaponData weaponData, Quaternion rotation)
        {

            if (SpawnAndUseThrowWeaponPoint == null) return null;

            GameObject newWeapon = WeaponFactory.CreateEntity(
                weaponData
                , SpawnAndUseThrowWeaponPoint.position
                , this.transform);

            if (newWeapon != null)
            {
                newWeapon.transform.rotation = rotation;
            }
            // 在手上的话 启用渲染
            LineRenderer lineRenderer = newWeapon.GetComponent<LineRenderer>();
            if(lineRenderer != null) { lineRenderer.enabled = true; }
            return newWeapon;
        }
        public GameObject SpawnOnHandWeapon(WeaponData weaponData, Quaternion rotation)
        {

            if (SpawnAndUseThrowWeaponPoint == null) return null;

            GameObject newWeapon = WeaponFactory.CreateEntity(
                weaponData
                , SpawnAndUseThrowWeaponPoint.position
                , this.transform);

            if (newWeapon != null)
            {
                newWeapon.transform.rotation = rotation;
            }

            return newWeapon;
        }

        private void OnPlayerUnderTableChg(object[] objs)
        {
            if (objs == null || objs.Length == 0) return;
            playerInfo.IsPlayerUnderDesk = (bool)objs[0];
        }

        #endregion

        #region 体力与生命

        /// <summary>
        /// 受伤
        /// </summary>
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (playerInfo.IsInvulnerable) return;

            playerInfo.CurrentHealth -= (int)damageInfo.damage;
            playerInfo.CurrentHealth = Mathf.Clamp(playerInfo.CurrentHealth, 0, playerInfo.MaxHealth);

            MsgCenter.SendMsg(MsgConst.ON_HEALTH_CHG, playerInfo.CurrentHealth);
    
            if (playerInfo.CurrentHealth <= 0)
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
            playerInfo.IsInvulnerable = true;
            playerInfo.CurrentStruggle = 0f;
        }

        public void Struggle()
        {
            playerInfo.CurrentStruggle += playerInfo.StruggleAmountOneTime;
            Debug.Log("<<<<<挣扎进度:" + playerInfo.CurrentStruggle);
            MsgCenter.SendMsg(MsgConst.ON_MANUAL_CIRCLE_PROGRESS_CHG, GlobalValue.CIRCLE_PROGRESS_STRUGGLE, playerInfo.CurrentStruggle / playerInfo.StruggleDemand);
            // 挣扎完成
            if (playerInfo.CurrentStruggle >= playerInfo.StruggleDemand)
            {
                EndStruggle();
            }
        }

        private void EndStruggle()
        {
            ChangeState(PlayerState.Idle);
            StartCoroutine(EndInvulnerabilityAfterTime(playerInfo.StruggleInvulnerabilityDuration));
        }

        private IEnumerator EndInvulnerabilityAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            playerInfo.IsInvulnerable = false;
        }
        
        /// <summary>
        /// 治疗
        /// </summary>
        /// <param name="healAmount">恢复值</param>
        public void Heal(float healAmount)
        {
            playerInfo.CurrentHealth += (int)healAmount;
            playerInfo.CurrentHealth = Mathf.Clamp(playerInfo.CurrentHealth, 0, playerInfo.MaxHealth);
            MsgCenter.SendMsg(MsgConst.ON_HEALTH_CHG, playerInfo.CurrentHealth);
        }

        public void Dead()
        {
            //TODO:临时测试
            MsgCenter.SendMsgAct(MsgConst.ON_PLAYER_DEAD);
            GameManager.Instance.GameFinish(false);
            Destroy(gameObject);
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
        public bool ConsumeStamina(float amount)
        {
            if (playerInfo.IsExhausted) return false;

            playerInfo.CurrentStamina -= amount;
            
            if (playerInfo.CurrentStamina <= 0)
            {
                playerInfo.CurrentStamina = 0;
                return false;
            }
            MsgCenter.SendMsg(MsgConst.ON_STAMINA_CHG, playerInfo.CurrentStamina / playerInfo.MaxStamina);
            if (playerInfo.CurrentStamina <= playerInfo.MaxStamina * playerInfo.RecoverThreshold)
            {
                playerInfo.IsExhausted = true;
            }

            if (playerInfo.CurrentStamina <= playerInfo.MaxStamina)
            {
                playerInfo.IsRecovering = true;
            }
            
            return true;
        }

        #endregion

        public bool GetCanPlaceTrap()
        {
            if (playerInfo.CurPreviewTrapGO == null) return false;
            return playerInfo.CurPreviewTrapGO.GetComponentInParent<TrapBase>().CanPlaceTrap;
        }

        public string GetSettingKey(InputActionType actionType, ControlType controlType)
        {
            return inputSettings.GetSettingKey(actionType, (int)controlType);
        }
    }
}