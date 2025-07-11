using BehaviorDesigner.Runtime.Tasks;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class CheckDizzy : BaseEnemyConditional
    {
        public float DizzyTime;
        public override TaskStatus OnUpdate()
        {
            if (enemy.CheckDizzyState())
            {
                UIHelper.Instance.ShowOneSign(new SignInfo(GlobalValue.SIGN_ICON_DIZZY_PATH, enemy.gameObject, DizzyTime));
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
