using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class Dizzy : BaseEnemyAction
    {
        public float DizzyTime;
        private float dizzyTimer;


        public override void OnStart()
        {
            dizzyTimer = 0;
            enemy.StopNav();
        }
        public override TaskStatus OnUpdate()
        {
            dizzyTimer += Time.deltaTime;
            if(dizzyTimer >= DizzyTime)
            {
                enemy.StartNav();
                enemy.SetDizzyState(false);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

    }
}
