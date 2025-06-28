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
                return TaskStatus.Success;
            }
            return TaskStatus.Running;

        }
        public override void OnEnd()
        {
            enemy.SetRoomCheckState(searchRoomType, true);
        }
    }


}
