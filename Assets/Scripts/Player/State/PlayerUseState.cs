using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerUseState : PlayerStateBase
    {
        private float useTimer;
        private bool hasPlaced;

        public override void Enter()
        {
            //player.PlayerAnimation("Throw");
            useTimer = 0f;
            hasPlaced = false;

            TryPlaceTrap();
        }

        public override void Update()
        {
            base.Update();

            // ������ɺ󷵻ؿ���״̬
            useTimer += Time.deltaTime;
            if (useTimer > 0.5f && hasPlaced)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }

        private void TryPlaceTrap()
        {
            if (PlayerBag.Instance._trapBag.Count == 0)
            {
                hasPlaced = true;
                return;
            }

            // ��ȡ��ǰѡ�е�����
            var trapToPlace = PlayerBag.Instance._trapBag[0];

            // �����������Ƴ�
            if (--trapToPlace.amount <= 0)
            {
                PlayerBag.Instance._trapBag.RemoveAt(0);
            }

            // �������λ��
            // ��Ҫ��������ϵͳ����
            Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;

            // ʵ��������Ԥ����
            GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, placePosition);
            if (newTrap != null)
            {
                newTrap.transform.rotation = player.transform.rotation;
                hasPlaced = true;
            }
        }
    }
}
