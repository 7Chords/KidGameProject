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

        //To do: ����������ҵ��ֳ���Ʒ���� ������һ����Use����
        public override void Enter()
        {
            useTimer = 0f;
            hasPlaced = false;

            Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;
            Quaternion rotation = player.transform.rotation;

            hasPlaced = PlayerBag.Instance.TryUseTrapFromTempBag(
                PlayerBag.Instance.SelectedTrapIndex,
                player,
                placePosition,
                rotation
            );

            if (!hasPlaced)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("�޷���������", player.gameObject));
                player.ChangeState(PlayerState.Idle);
            }
        }

        public override void Update()
        {
            base.Update();
            
            useTimer += Time.deltaTime;
            if (useTimer > 0.5f && hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }
}