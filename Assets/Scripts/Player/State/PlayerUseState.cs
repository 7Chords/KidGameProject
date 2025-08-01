using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerUseState : PlayerStateBase
    {
        private float useTimer;
        private bool canUseItem;
        private ISlotInfo selectedItem;

        private Vector3 position;
        private Quaternion rotation;


        public override void Enter()
        {
            useTimer = -0.1f;
            canUseItem = false;

            selectedItem = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (selectedItem == null)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("未选中道具", player.gameObject));
                player.ChangeState(PlayerState.Idle);
                return;
            }

            position = player.transform.position + player.transform.forward + Vector3.up;
            rotation = player.transform.rotation;

            switch (selectedItem.ItemData.UseItemType)
            {
                case UseItemType.trap:
                    canUseItem = player.GetCanPlaceTrap();
                    break;
                case UseItemType.weapon:
                    canUseItem = PlayerBag.Instance.UseWeapon(selectedItem, position, rotation);
                    break;
                case UseItemType.food:
                    canUseItem = PlayerBag.Instance.UseFood(selectedItem, player);
                    break;
                case UseItemType.Material:
                    canUseItem = PlayerBag.Instance.UseMaterial(selectedItem, player);
                    break;
                default:
                    Debug.LogError("未知道具类型，使用失败");
                    break;
            }

            if (!canUseItem)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("使用失败", player.gameObject));
                player.ChangeState(PlayerState.Idle);
            }
            else if( canUseItem && selectedItem.ItemData.UseItemType == UseItemType.trap)
            {
                UIHelper.Instance.ShowCircleProgress(player.gameObject, (selectedItem.ItemData as TrapData).placeTime);
            }
        }


        public override void Update()
        {
            base.Update();

            Vector2 inputVal = player.InputSettings.MoveDir();
            // 检测玩家的输入
            if (inputVal != Vector2.zero)
            {
                UIHelper.Instance.DestoryCurrentCircleProgress();
                // 切换状态
                player.ChangeState(PlayerState.Move);
                return;
            }
            switch (selectedItem.ItemData.UseItemType)
            {
                case UseItemType.trap:
                    //陷阱使用需要计时
                    useTimer += Time.deltaTime;
                    if (useTimer > (selectedItem.ItemData as TrapData).placeTime && canUseItem)
                    {
                        GameObject newTrap = TrapFactory.CreateEntity((selectedItem.ItemData as TrapData), position);
                        if (newTrap != null)
                        {
                            newTrap.transform.rotation = rotation;
                            PlayerBag.Instance.DeleteItemInCombineBag(selectedItem.ItemData.Id, 1);
                        }
                        player.ChangeState(PlayerState.Idle);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}