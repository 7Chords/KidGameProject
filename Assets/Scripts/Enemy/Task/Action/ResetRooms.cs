using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class ResetRooms : BaseEnemyAction
    {
        public override void OnStart()
        {
            enemy.ResetAllRoomsCheckState();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
