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

            // 获取当前选中的陷阱
            var trapToPlace = PlayerBag.Instance._trapBag[0];

            // 减少数量或移除
            if (--trapToPlace.amount <= 0)
            {
                PlayerBag.Instance._trapBag.RemoveAt(0);
            }

            //// 计算放置位置
            //// 需要根据网格系统调整
            //Vector3 placePosition = player.transform.position + player.transform.forward + Vector3.up;

            //// 实例化陷阱预制体
            //GameObject newTrap = TrapFactory.Create(trapToPlace.trapData, placePosition);
            //if (newTrap != null)
            //{
            //    newTrap.transform.rotation = player.transform.rotation;
            //}
        }
    }
}

