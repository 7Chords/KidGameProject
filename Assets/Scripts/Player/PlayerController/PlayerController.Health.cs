using System.Collections;
using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
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
    }
}