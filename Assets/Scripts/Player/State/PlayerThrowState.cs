using UnityEngine;

namespace KidGame.Core
{
    public class PlayerThrowState : PlayerStateBase
    {


        public override void Enter()
        {
            //player.PlayerAnimation("Throw");

            TryPlaceTrap();
            player.ChangeState(PlayerState.Idle);
        }

        public override void Update()
        {
            base.Update();
        }

        private void TryPlaceTrap()
        {
            if (PlayerBag.Instance._trapBag.Count == 0)
            {
                return;
            }

            // ��ȡ��ǰѡ�е�����
            var trapToPlace = PlayerBag.Instance._trapBag[0];

            // �����������Ƴ�
            if (--trapToPlace.amount <= 0)
            {
                PlayerBag.Instance._trapBag.RemoveAt(0);
            }

            //// �������λ��
            //// ��Ҫ��������ϵͳ����
            //Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;

            //// ʵ��������Ԥ����
            //GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, placePosition);
            //if (newTrap != null)
            //{
            //    newTrap.transform.rotation = player.transform.rotation;
            //}
        }
    }
}

