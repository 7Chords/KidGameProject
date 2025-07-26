using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class SearchNearestRoom : BaseEnemyAction
    {
        private RoomType searchRoomType;
        public override void OnStart()
        {
            searchRoomType = enemy.GoNearestUnCheckRoom();
        }

        public override TaskStatus OnUpdate()
        {
            if (enemy.CheckArriveDestination())
            {
                enemy.SetRoomCheckState(searchRoomType, true);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;

        }
        //public override void OnEnd()
        //{
        //    enemy.SetRoomCheckState(searchRoomType, true);
        //}
    }


}
