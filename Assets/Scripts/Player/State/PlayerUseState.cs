using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public enum UseItemType
    {
        trap,
        weapon,
        food,
        Material,
        nothing
    }


    public class PlayerUseState : PlayerStateBase
    {
        private float useTimer;
        private bool hasPlaced;

        public override void Enter()
        {
            useTimer = 0f;
            hasPlaced = false;

            ISlotInfo selectedItem = PlayerBag.Instance.GetSelectedTempItem();
            if (selectedItem == null)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("未选中道具", player.gameObject));
                player.ChangeState(PlayerState.Idle);
                return;
            }

            Vector3 position = player.transform.position + player.transform.forward + Vector3.up;
            Quaternion rotation = player.transform.rotation;

            switch (selectedItem.ItemData.UseItemType)
            {
                case UseItemType.trap:
                    hasPlaced = PlayerBag.Instance.UseTrap(selectedItem, position, rotation);
                    break;
                case UseItemType.weapon:
                    hasPlaced = PlayerBag.Instance.UseWeapon(selectedItem, player, position);
                    break;
                case UseItemType.food:
                    hasPlaced = PlayerBag.Instance.UseFood(selectedItem, player);
                    break;
                case UseItemType.Material:
                    hasPlaced = PlayerBag.Instance.UseMaterial(selectedItem, player);
                    break;
                default:
                    UIHelper.Instance.ShowOneTip(new TipInfo("未知道具类型", player.gameObject));
                    break;
            }

            if (!hasPlaced)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("使用失败", player.gameObject));
                player.ChangeState(PlayerState.Idle);
            }
        }


        public override void Update()
        {
            base.Update();
            
            useTimer += Time.deltaTime;
            if (useTimer > 2f && hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }
}