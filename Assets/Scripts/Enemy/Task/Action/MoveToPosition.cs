 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class MoveToPosition : BaseEnemyAction
    {
        public Vector3 targetPosition;
        public override void OnAwake()
        {
            base.OnAwake();
            enemy.SetMoveTarget(targetPosition);
        }
        
    }
}