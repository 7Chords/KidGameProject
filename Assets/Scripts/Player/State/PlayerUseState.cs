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
                UIHelper.Instance.ShowOneTip(new TipInfo("δѡ�е���", player.transform.position));
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

                    WeaponSlotInfo weaponItem = (WeaponSlotInfo)selectedItem;

                    //����ǳ���ʹ�����͵�
                    if (weaponItem.weaponData.longOrShortPress == 0)
                    {
                        canUseItem = PlayerBag.Instance.UseWeaponLongPress(selectedItem);
                    }
                    else
                    {
                        canUseItem = PlayerBag.Instance.UseWeaponShortClick(selectedItem
                            , PlayerController.Instance.SpawnAndUseThrowWeaponPoint.position
                            , rotation);
                    }
                    break;
                case UseItemType.food:
                    canUseItem = PlayerBag.Instance.UseFood(selectedItem, player);
                    break;
                case UseItemType.Material:
                    canUseItem = PlayerBag.Instance.UseMaterial(selectedItem, player);
                    break;
                default:
                    Debug.LogError("δ֪�������ͣ�ʹ��ʧ��");
                    break;
            }

            if (!canUseItem)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("ʹ��ʧ��", player.transform.position));
                player.ChangeState(PlayerState.Idle);
            }
            else if( canUseItem && selectedItem.ItemData.UseItemType == UseItemType.trap)
            {
                UIHelper.Instance.ShowCircleProgress(GlobalValue.CIRCLE_PROGRESS_PLACE_TRAP,
                    CircleProgressType.Auto,player.gameObject, (selectedItem.ItemData as TrapData).placeTime);
            }
        }


        public override void Update()
        {
            base.Update();

            Vector2 inputVal = player.InputSettings.MoveDir();
            // �����ҵ�����
            if (inputVal != Vector2.zero)
            {
                // �л�״̬
                player.ChangeState(PlayerState.Move);
                return;
            }
            switch (selectedItem.ItemData.UseItemType)
            {
                case UseItemType.trap:
                    //����ʹ����Ҫ��ʱ
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

        public override void Exit()
        {
            base.Exit();
            UIHelper.Instance.DestoryCircleProgress(GlobalValue.CIRCLE_PROGRESS_PLACE_TRAP);
        }
    }
}